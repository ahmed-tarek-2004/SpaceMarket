using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.Cart;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Configurations;
using Ecommerce.Utilities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecommerce.DataAccess.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly ResponseHandler _responseHandler;
        private readonly ILogger<CartService> _logger;
        private readonly CommissionSettings _commissionSettings;

        public CartService(
            ApplicationDbContext context,
            ResponseHandler responseHandler,
            ILogger<CartService> logger,
            IOptions<CommissionSettings> commissionOptions)
        {
            _context = context;
            _responseHandler = responseHandler;
            _logger = logger;
            _commissionSettings = commissionOptions?.Value ?? new CommissionSettings { RatePercent = 0m };
        }

        #region Adding To Cart Region
        public async Task<Response<CartResponse>> AddingToCartAsync(string clientId, AddingToCartRequest request)
        {
            _logger.LogInformation("Attempting to add item (ServiceId: {ServiceId}, DatasetId: {DataSetId}) to cart for ClientId: {ClientId}",
                request.ServiceId, request.DataSetId, clientId);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                bool isDataset = request.DataSetId.HasValue && request.DataSetId.Value != Guid.Empty;
                bool isService = request.ServiceId.HasValue && request.ServiceId.Value != Guid.Empty;

                if (isDataset == isService) // must provide exactly one
                    return _responseHandler.BadRequest<CartResponse>("Provide exactly one of serviceId or datasetId.");

                decimal price;
                Guid itemId;

                if (isDataset)
                {
                    var dataset = await _context.Datasets
                        .Include(d => d.Provider)
                        .FirstOrDefaultAsync(d => d.Id == request.DataSetId && !d.IsDeleted && d.Status == ServiceStatus.Active);

                    if (dataset == null)
                        return _responseHandler.NotFound<CartResponse>("Dataset not found, inactive, or not approved.");

                    price = dataset.Price;
                    itemId = dataset.Id;
                }
                else
                {
                    var service = await _context.Services
                        .Include(s => s.Provider)
                        .FirstOrDefaultAsync(s => s.Id == request.ServiceId && !s.IsDeleted && s.Status == ServiceStatus.Active);

                    if (service == null)
                        return _responseHandler.NotFound<CartResponse>("Service not found, inactive, or not approved.");

                    price = service.Price;
                    itemId = service.Id;
                }

                // get or create cart
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Service)
                        .ThenInclude(s => s.Provider)
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Dataset)
                        .ThenInclude(d => d.Provider)
                    .FirstOrDefaultAsync(c => c.ClientId == clientId);

                if (cart == null)
                {
                    cart = new Entities.Models.Cart
                    {
                        Id = Guid.NewGuid(),
                        ClientId = clientId,
                        CartItems = new List<CartItem>(),
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Carts.Add(cart);
                }

                cart.CartItems ??= new List<CartItem>();

                // check duplicates (no quantity allowed)
                var exists = cart.CartItems.FirstOrDefault(ci =>
                    (isDataset && ci.DatasetId.HasValue && ci.DatasetId.Value == itemId) ||
                    (!isDataset && ci.ServiceId.HasValue && ci.ServiceId.Value == itemId));

                if (exists != null)
                {
                    // ensure navigation properties are loaded for response
                    var existingResponse = BuildCartResponse(cart);
                    return _responseHandler.Success(existingResponse, "Item already exists in cart.");
                }

                var newCartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ServiceId = isService ? itemId : null,
                    DatasetId = isDataset ? itemId : null,
                    PriceSnapshot = price,
                    ClientId=clientId,
                    CreatedAt = DateTime.UtcNow
                };

                // add both to context and to in-memory collection so response is accurate
                await _context.CartItems.AddAsync(newCartItem);
                cart.CartItems.Add(newCartItem);

                cart.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = BuildCartResponse(cart);
                return _responseHandler.Success(response, "Item added to cart successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while adding item to cart for ClientId: {ClientId}", clientId);
                return _responseHandler.InternalServerError<CartResponse>("An error occurred while adding item to cart.");
            }
        }
        #endregion

        #region Get Cart As A Client
        public async Task<Response<CartResponse>> GetCartAsync(string clientId)
        {
            _logger.LogInformation("Retrieving cart for ClientId: {ClientId}", clientId);

            try
            {
                var cart = await _context.Carts
                    .AsNoTracking()
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Service)
                        .ThenInclude(s => s.Provider)
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Dataset)
                        .ThenInclude(d => d.Provider)
                    .FirstOrDefaultAsync(c => c.ClientId == clientId);

                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                {
                    return _responseHandler.Success(new CartResponse
                    {
                        CartId = Guid.Empty, // fixed: do NOT return clientId here
                        TotalItems = 0,
                        TotalPrice = 0,
                        TotalCommission = 0,
                        Items = new List<CartItemResponse>()
                    }, "Cart is empty (Has 0 Cart Item/s).");
                }

                var response = BuildCartResponse(cart);
                return _responseHandler.Success(response, "Cart retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving cart for ClientId: {ClientId}", clientId);
                return _responseHandler.InternalServerError<CartResponse>("An error occurred while retrieving cart.");
            }
        }
        #endregion

        #region Clear Cart
        public async Task<Response<CartResponse>> ClearCartItemsAsync(string clientId)
        {
            _logger.LogInformation("Attempting to clear cart for ClientId: {ClientId}", clientId);
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.ClientId == clientId);

                if (cart == null)
                    return _responseHandler.NotFound<CartResponse>("Cart not found.");

                if (cart.CartItems == null || !cart.CartItems.Any())
                    return _responseHandler.NotFound<CartResponse>("Cart is already empty.");

                // remove from both context and in-memory collection
                _context.CartItems.RemoveRange(cart.CartItems);
                cart.CartItems.Clear();
                cart.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return _responseHandler.Success(new CartResponse
                {
                    CartId = cart.Id,
                    TotalItems = 0,
                    TotalPrice = 0,
                    TotalCommission = 0,
                    Items = new List<CartItemResponse>()
                }, "Cart cleared successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while clearing cart for ClientId: {ClientId}", clientId);
                return _responseHandler.InternalServerError<CartResponse>("An error occurred while clearing cart.");
            }
        }
        #endregion

        #region Remove Cart Item
        public async Task<Response<CartResponse>> RemoveCartItemAsync(string clientId, Guid cartItemId)
        {
            _logger.LogInformation("Attempting to remove CartItemId: {CartItemId} for ClientId: {ClientId}", cartItemId, clientId);
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Service)
                        .ThenInclude(s => s.Provider)
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Dataset)
                        .ThenInclude(d => d.Provider)
                    .FirstOrDefaultAsync(c => c.ClientId == clientId);

                if (cart == null)
                    return _responseHandler.NotFound<CartResponse>("Cart not found.");

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
                if (cartItem == null)
                    return _responseHandler.NotFound<CartResponse>("Cart item not found.");

                // remove from in-memory collection and context to keep response accurate
                cart.CartItems.Remove(cartItem);
                _context.CartItems.Remove(cartItem);

                cart.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = BuildCartResponse(cart);
                return _responseHandler.Success(response, "Cart item removed successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while removing cart item for ClientId: {ClientId}", clientId);
                return _responseHandler.InternalServerError<CartResponse>("An error occurred while removing cart item.");
            }
        }
        #endregion

        #region Helper
        private CartResponse BuildCartResponse(Entities.Models.Cart cart)
        {
            var commissionRate = _commissionSettings?.RatePercent ?? 0m;
            var cartItems = new List<CartItemResponse>();
            decimal totalPrice = 0m;
            decimal totalCommission = 0m;

            foreach (var ci in cart.CartItems)
            {
                bool isDataset = ci.DatasetId.HasValue && ci.DatasetId.Value != Guid.Empty;
                decimal unitPrice = ci.PriceSnapshot > 0 ? ci.PriceSnapshot :
                                     (isDataset ? (ci.Dataset?.Price ?? 0m) : (ci.Service?.Price ?? 0m));

                decimal commissionAmount = 0m;
                if (commissionRate > 0m)
                    commissionAmount = Math.Round(unitPrice * commissionRate / 100m, 2);

                decimal providerAmount = unitPrice - commissionAmount;

                var item = new CartItemResponse
                {
                    CartItemId = ci.Id,
                    ItemId = isDataset ? ci.DatasetId : ci.ServiceId,
                    ItemType = isDataset ? "dataset" : "service",
                    Title = isDataset ? (ci.Dataset?.Title ?? "Dataset") : (ci.Service?.Title ?? "Service"),
                    ProviderName = isDataset
                        ? (ci.Dataset?.Provider?.CompanyName ?? "Unknown")
                        : (ci.Service?.Provider?.CompanyName ?? "Unknown"),
                    UnitPrice = unitPrice,
                    CommissionPercent = commissionRate,
                    CommissionAmount = commissionAmount,
                    ProviderAmount = providerAmount,
                    ImageUrl = isDataset ? (ci.Dataset?.ThumbnailUrl ?? "") : (ci.Service?.ImagesUrl ?? ""),
                    Total = unitPrice
                };

                cartItems.Add(item);
                totalPrice += unitPrice;
                totalCommission += commissionAmount;
            }

            return new CartResponse
            {
                CartId = cart.Id,
                Items = cartItems,
                TotalItems = cartItems.Count,
                TotalPrice = totalPrice,
                TotalCommission = totalCommission
            };
        }
        #endregion
    }
}
