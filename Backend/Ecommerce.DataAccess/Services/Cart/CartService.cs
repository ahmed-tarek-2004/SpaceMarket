using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.Cart;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Shared.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly ResponseHandler _responseHandler;
        private readonly ILogger<CartService> _logger;

        public CartService(
            ApplicationDbContext context,
            ResponseHandler responseHandler,
            ILogger<CartService> logger)
        {
            _context = context;
            _responseHandler = responseHandler;
            _logger = logger;
        }

        #region Adding To Cart Region
        public async Task<Response<CartResponse>> AddingToCartAsync(string clientId, AddingToCartRequest request)
        {
            _logger.LogInformation("Attempting to add Service {ServiceId} to cart for ClientId: {ClientId}", request.ServiceId, clientId);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
               
                var service = await _context
                    .Services
                    .Include(s => s.Provider)
                    .FirstOrDefaultAsync(s => s.Id == request.ServiceId);

                if (service == null)
                {
                    _logger.LogWarning("Service not found or inactive or not approved. Service Id: {ServiceId}", request.ServiceId);
                    return _responseHandler.NotFound<CartResponse>(
                        "Service not found, inactive, or not approved by admin."
                    );
                }

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Service)
                    .ThenInclude(s => s.Provider)
                    .FirstOrDefaultAsync(c => c.ClientId == clientId);

                if (cart == null)
                {
                    cart = new Entities.Models.Cart
                    {
                        Id = Guid.NewGuid(),
                        ClientId = clientId,
                        CartItems = new List<CartItem>()
                    };
                    _context.Carts.Add(cart);
                }

                cart.CartItems ??= new List<CartItem>();
                _logger.LogInformation("CartItems Count: {Count}", cart.CartItems.Count);

                // Check if the service already exists in cart
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ServiceId == request.ServiceId);
                if (cartItem == null)
                {
                    _logger.LogInformation("Adding new CartItem to Cart.");
                    cartItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        Service=service,
                        ServiceId = request.ServiceId,
                        PriceSnapshot = service.Price,
                        Quantity = request.Quantity,
                        DatasetId = request.DataSetId
                    };
                    _context.CartItems.Add(cartItem);
                }
                else
                {
                    _logger.LogInformation("Increasing quantity for existing CartItem: {ServiceId}", request.ServiceId);
                    cartItem.Quantity += request.Quantity;
                    // Optionally update snapshot price if needed:
                    // cartItem.PriceSnapshot = service.Price;
                }

                cart.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Build response using DTOs
                var response = await BuildCartResponse(cart);

                _logger.LogInformation("Service {ServiceId} added successfully for ClientId: {ClientId}", request.ServiceId, clientId);
                return _responseHandler.Success<CartResponse>(response, "Service added to cart successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while adding Service {ServiceId} to cart for ClientId: {ClientId}", request.ServiceId, clientId);
                return _responseHandler.InternalServerError<CartResponse>("An error occurred while adding Service to cart.");
            }
        }
        #endregion



        #region Get Cart As A Client
        public async Task<Response<CartResponse>> GetCartAsync(string clientId)
        {

            _logger.LogInformation("Retrieving cart for ClientId: {clientId}", clientId);

            try
            {
                var cart = await _context.Carts
                    .AsNoTracking()
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Service)
                    .ThenInclude(s => s.Provider)
                    .FirstOrDefaultAsync(c => c.ClientId == clientId);

                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                {
                    _logger.LogInformation("Cart is empty for Client Id: {clientId}", clientId);
                    return _responseHandler.Success(new CartResponse
                    {
                        CartId = clientId,
                        TotalItems = 0,
                        TotalPrice = 0,
                        CartItems = new List<CartItemResponse>()
                    }, "Cart is empty.");
                }
                _logger.LogDebug("Cart contains {CartItemCount} items for ClientId: {clientId}", cart.CartItems.Count, clientId);
                _logger.LogInformation($"Price For item is :{cart.CartItems.FirstOrDefault().PriceSnapshot}");
                var response = await BuildCartResponse(cart);

                _logger.LogInformation("Cart retrieved successfully for ClientId :{clientId}", clientId);
                return _responseHandler.Success(response, "Cart retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving cart for ClientId: {clientId}", clientId);
                return _responseHandler.InternalServerError<CartResponse>("An error occurred while retrieving cart.");
            }
        }
        #endregion


        #region Update CartItem
        public async Task<Response<CartResponse>> UpdateCartItemQuantityAsync(string clientId, UpdateCartItemRequest request)
        {
            _logger.LogInformation("Attempting to update quantity for CartItemId: {CartItemId} to {Quantity} for ClientId: {ClientId}", request.CartItemId, request.Quantity, clientId);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Service)
                    .ThenInclude(s => s.Provider)
                    .FirstOrDefaultAsync(c => c.ClientId == clientId);

                if (cart == null)
                {
                    _logger.LogWarning("Cart not found for clientId: {clientId}", clientId);
                    return _responseHandler.NotFound<CartResponse>("Cart not found.");
                }

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == request.CartItemId);
                if (cartItem == null)
                {
                    _logger.LogWarning("CartItem not found. CartItemId: {CartItemId}", request.CartItemId);
                    return _responseHandler.NotFound<CartResponse>("Cart item not found.");
                }
                if (request.Quantity < 0)
                {
                    _logger.LogWarning("Not Allowed Quantity",
                        cartItem.ServiceId, request.Quantity, cartItem.Quantity);

                    return _responseHandler.BadRequest<CartResponse>("Not Allowed Quantity");
                }
                else if (request.Quantity == 0)
                {
                    _logger.LogInformation($"Start Deleting CartItem cartItem:{cartItem.Id} from Database");
                    cart.UpdatedAt = DateTime.UtcNow;
                    //var temp = cartItem;
                    _context.CartItems.Remove(cartItem);
                    // await _context.SaveChangesAsync();
                    _logger.LogInformation("Cart item quantity updated successfully to Zero And Delete From Cart.");

                    // var responseForDeleted = await BuildCartResponse(cart);

                    //await transaction.CommitAsync();
                    // return _responseHandler.Success(responseForDeleted, "Cart item quantity updated successfully to Zero And Delete From Cart.");

                }
                else
                {
                    cartItem.Quantity = request.Quantity;
                    cart.UpdatedAt = DateTime.UtcNow;
                    _logger.LogInformation("Cart item quantity updated successfully to Zero And Delete From Cart.");
                }
                //cart.CartItems.FirstOrDefault(c => c.Id == cartItem.Id).Quantity = cartItem.Quantity;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();


                var response = await BuildCartResponse(cart);
                _logger.LogInformation("Cart item quantity updated successfully for clientId: {clientId}", clientId);
                return _responseHandler.Success(response, "Cart item quantity updated successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while updating cart item quantity for clientId: {clientId}", clientId);
                return _responseHandler.InternalServerError<CartResponse>("An error occurred while updating cart item quantity.");
            }
        }

        #endregion

        #region Clear Cart
        public async Task<Response<CartResponse>> ClearCartItemsAsync(string clientId)
        {
            _logger.LogInformation("Attempting to remove All CartItems for clientId: {clientId}", clientId);
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.ClientId == clientId);

                if (cart == null)
                {
                    _logger.LogWarning("Cart not found for clientId: {clientId}", clientId);
                    return _responseHandler.NotFound<CartResponse>("Cart not found.");
                }

                var cartItems = cart.CartItems;
                if (cartItems == null)
                {
                    _logger.LogWarning("No CartItems assigned yet");
                    return _responseHandler.NotFound<CartResponse>("Cart is empty.");
                }

                _context.RemoveRange(cartItems);
                cart.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Cart cleared successfully for clientId: {clientId}", clientId);

                return _responseHandler.Success(new CartResponse
                {
                    CartId = cart.Id.ToString(),
                    TotalItems = 0,
                    TotalPrice = 0,
                    CartItems = new List<CartItemResponse>()
                }, "Cart cleared successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while removing cart item for clientId: {clientId}", clientId);
                return _responseHandler.InternalServerError<CartResponse>("An error occurred while removing cart item.");
            }
        }
        #endregion

        #region Remove Cart Item
        public async Task<Response<CartResponse>> RemoveCartItemAsync(string clientId, Guid cartItemId)
        {
            _logger.LogInformation("Attempting to remove CartItemId: {CartItemId} for clientId: {clientId}", cartItemId, clientId);
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Service)
                    .ThenInclude(s => s.Provider)
                    .FirstOrDefaultAsync(c => c.ClientId == clientId);

                if (cart == null)
                {
                    _logger.LogWarning("Cart not found for clientId: {clientId}", clientId);
                    return _responseHandler.NotFound<CartResponse>("Cart not found.");
                }

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
                if (cartItem == null)
                {
                    _logger.LogWarning("CartItem not found. CartItemId: {CartItemId}", cartItemId);
                    return _responseHandler.NotFound<CartResponse>("Cart item not found.");
                }

                cart.CartItems.Remove(cartItem);
                cart.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = await BuildCartResponse(cart);
                _logger.LogInformation("Cart item removed successfully for clientId: {clientId}", clientId);
                return _responseHandler.Success(response, "Cart item removed successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while removing cart item for clientId: {clientId}", clientId);
                return _responseHandler.InternalServerError<CartResponse>("An error occurred while removing cart item.");
            }
        }

        #endregion


        #region Helper
        private async Task<CartResponse> BuildCartResponse(Entities.Models.Cart cart)
        {

            var cartItems = cart.CartItems.Select(ci => new CartItemResponse
            {
                CartItemId = ci.Id,
                ServiceId = ci.ServiceId,
                ServiceTitle = ci.Service?.Title??"Empty",
                ProviderName = ci.Service?.Provider?.CompanyName??"Empty",
                UnitPrice = ci.Service?.Price??ci.PriceSnapshot,
                Quantity = ci.Quantity,
                ImageUrl = ci.Service?.ImagesUrlJson??"Empty",
                Total = ci.Service.Price * ci.Quantity,
            }).ToList();

            return new CartResponse
            {
                CartItems = cartItems,
                CartId = cart.Id.ToString(),
                TotalItems = cart.CartItems.Count(),
                TotalPrice = cartItems.Sum(ci => ci.Total),
            };
        }
        #endregion
    }
}
