using Ecommerce.DataAccess.ApplicationContext;
using Ecommerce.Entities.DTO.Order;
using Ecommerce.Entities.DTO.Shared;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Shared;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.DataAccess.Services.Order;
public class OrderService(
        ApplicationDbContext context,
        ILogger<OrderService> logger,
        ResponseHandler responseHandler
    ) : IOrderService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<OrderService> _logger = logger;
    private readonly ResponseHandler _responseHandler = responseHandler;

    public async Task<Response<PaginatedList<OrderResponse>>> GetAllOrdersAsync(string userId, string role, OrderFilters<OrderSorting> filters, CancellationToken cancellationToken)
    {

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("GetAllOrdersAsync: Invalid userId");
            return _responseHandler.BadRequest<PaginatedList<OrderResponse>>("Invalid client ID");
        }


        _logger.LogInformation("Fetching orders for client {ClientId} with filters: {@Filters}", userId, filters);

        var query = Enumerable.Empty<Entities.Models.Order>().AsQueryable();

        if (role.ToLower() == "client")
        {
            query = _context.Orders
            .Where(o => o.ClientId == userId && !o.IsDeleted);
        }
        else if (role.ToLower() == "ServiceProvider")
        {
            query = _context.Orders
                .Where(o => o.Item.Service.ProviderId == userId && !o.IsDeleted);
        }
        else
        {
            _logger.LogWarning("GetAllOrdersAsync: Unauthorized role {Role}", role);
            return _responseHandler.Unauthorized<PaginatedList<OrderResponse>>("Unauthorized access");
        }


        var filteredList = FilterForUser(query, filters, userId);

        var source = filteredList.Include(o => o.Item)
                .Select(o => new OrderResponse
                {
                    Id = o.Id,
                    Amount = o.Amount,
                    Commission = o.Commission,
                    Status = o.Status.ToString(),
                    OrderItem = new OrderItemResponse
                    {
                        ItemId = o.Item.ServiceId != null && o.Item.ServiceId != Guid.Empty
                                                                                ? o.Item.ServiceId
                                                                                : o.Item.DatasetId,

                        PriceSnapshot = o.Item.PriceSnapshot,
                        Id = o.Item.Id,
                    }
                })
                .AsNoTracking().AsQueryable();


        var orders = await PaginatedList<OrderResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

        _logger.LogInformation("Fetched {Count} orders for client {ClientId} on page {PageNumber} with page size {PageSize}",
            orders.Items.Count, userId, filters.PageNumber, filters.PageSize);

        return _responseHandler.Success(orders, "Orders fetched successfully");



    }

    public async Task<Response<OrderResponse>> GetOrderByIdAsync(string clientId, Guid orderId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(clientId))
        {
            _logger.LogWarning("GetClientOrdersAsync: Invalid clientId");
            return _responseHandler.BadRequest<OrderResponse>("Invalid client ID");
        }


        _logger.LogInformation("Fetching order with id {OrderId} for client {ClientId}", orderId, clientId);


        var order = await _context.Orders
            .Include(o => o.Item)
            .FirstOrDefaultAsync(o => o.ClientId == clientId && o.Id == orderId && !o.IsDeleted, cancellationToken);

        if (order is null)
        {
            _logger.LogWarning("Order with id {OrderId} not found for client {ClientId}", orderId, clientId);
            return _responseHandler.NotFound<OrderResponse>("Order not found");
        }

        var orderResponse = new OrderResponse
        {
            Id = order.Id,
            Amount = order.Amount,
            Commission = order.Commission,
            Status = order.Status.ToString(),
            OrderItem = new OrderItemResponse
            {
                ItemId = string.IsNullOrEmpty(order.Item.ServiceId.ToString()) ? order.Item.ServiceId : order.Item.DatasetId,
                PriceSnapshot = order.Item.PriceSnapshot,
                Id = order.Item.Id,
            }
        };

        _logger.LogInformation("Fetched order {OrderId} for client {ClientId}}",
            orderId, clientId);

        return _responseHandler.Success<OrderResponse>(orderResponse, "Orders fetched successfully");
    }

    public async Task<Response<OrderResponse>> CreateOrderAsync(string clientId, OrderRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(clientId))
        {
            _logger.LogWarning("CreateOrderAsync: Invalid clientId");
            return _responseHandler.BadRequest<OrderResponse>("Invalid client ID");
        }

        _logger.LogInformation("Creating order for client {ClientId}", clientId);

        OrderItem orderItem = null;
        string providerId = null;

        // Validate item
        if (request.OrderItem.Type == ItemType.Service)
        {
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == request.OrderItem.ItemId && !s.IsDeleted, cancellationToken);

            if (service == null)
            {
                _logger.LogWarning("Invalid service ID {ServiceId} for client {ClientId}", request.OrderItem.ItemId, clientId);
                return _responseHandler.BadRequest<OrderResponse>("Invalid service ID.");
            }

            orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                ServiceId = service.Id,
                PriceSnapshot = service.Price,
            };

            providerId = service.ProviderId;
        }
        else if (request.OrderItem.Type == ItemType.Dataset)
        {
            var dataset = await _context.Datasets
                .FirstOrDefaultAsync(d => d.Id == request.OrderItem.ItemId && !d.IsDeleted, cancellationToken);

            if (dataset == null)
            {
                _logger.LogWarning("Invalid dataset ID {DatasetId} for client {ClientId}", request.OrderItem.ItemId, clientId);
                return _responseHandler.BadRequest<OrderResponse>("Invalid dataset ID.");
            }

            orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                DatasetId = dataset.Id,
                PriceSnapshot = dataset.Price,
            };

            providerId = dataset.ProviderId;
        }
        else
        {
            _logger.LogWarning("Invalid item type {ItemType} for client {ClientId}", request.OrderItem.Type, clientId);
            return _responseHandler.BadRequest<OrderResponse>("Invalid item type.");
        }


        // Create order
        var order = new Entities.Models.Order
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            Amount = orderItem!.PriceSnapshot,
            Commission = (orderItem.PriceSnapshot) * 0.1m,
            Status = OrderStatus.PendingPayment,
            CreatedAt = DateTime.UtcNow,
            Item = orderItem
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        // Map to response
        var orderResponse = new OrderResponse
        {
            Id = order.Id,
            ClientId = clientId,
            ProviderId = providerId!,
            Amount = order.Amount,
            Commission = order.Commission,
            OrderDate = order.CreatedAt,
            Status = order.Status.ToString(),
            OrderItem = new OrderItemResponse
            {
                Id = order.Item.Id,
                ItemId = order.Item.ServiceId ?? order.Item.DatasetId ?? Guid.Empty,
                Type = request.OrderItem.Type,
                PriceSnapshot = order.Item.PriceSnapshot
            }
        };

        _logger.LogInformation("Created order {OrderId} for client {ClientId}", order.Id, clientId);

        return _responseHandler.Success(orderResponse, "Order created successfully.");
    }


    public async Task<Response<OrderResponse>> UpdateOrderAsync(string clientId, UpdateOrderRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(clientId))
        {
            _logger.LogWarning("UpdateOrderAsync: Invalid clientId");
            return _responseHandler.BadRequest<OrderResponse>("Invalid client ID");
        }

        _logger.LogInformation("Updating order {OrderId} for client {ClientId}", request.OrderId, clientId);

        var order = await _context.Orders
            .Include(o => o.Item)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.ClientId == clientId, cancellationToken);

        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found for client {ClientId}", request.OrderId, clientId);
            return _responseHandler.NotFound<OrderResponse>("Order not found");
        }

        if (order.Status != OrderStatus.PendingPayment)
        {
            _logger.LogWarning("Cannot update order {OrderId} because status is {Status}", request.OrderId, order.Status);
            return _responseHandler.BadRequest<OrderResponse>("Only orders in PendingPayment status can be updated");
        }

        if (request.OrderItem is null)
        {
            return _responseHandler.BadRequest<OrderResponse>("Order must contain exactly one item.");
        }

        var itemRequest = request.OrderItem;
        Service? service = null;
        Dataset? dataset = null;

        if (itemRequest.Type == ItemType.Service)
        {
            service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == itemRequest.ItemId && !s.IsDeleted, cancellationToken);
            if (service == null)
            {
                return _responseHandler.BadRequest<OrderResponse>("Invalid service ID.");
            }
        }
        else if (itemRequest.Type == ItemType.Dataset)
        {
            dataset = await _context.Datasets
                .FirstOrDefaultAsync(d => d.Id == itemRequest.ItemId && !d.IsDeleted, cancellationToken);
            if (dataset == null)
            {
                return _responseHandler.BadRequest<OrderResponse>("Invalid dataset ID.");
            }
        }

        // Update Item
        order.Item.ServiceId = service?.Id;
        order.Item.DatasetId = dataset?.Id;
        order.Item.PriceSnapshot = itemRequest.PriceSnapshot;

        // Update Order
        order.Amount = itemRequest.PriceSnapshot;
        order.Commission = itemRequest.PriceSnapshot * 0.1m;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var orderResponse = new OrderResponse
        {
            Id = order.Id,
            ClientId = order.ClientId,
            Amount = order.Amount,
            Commission = order.Commission,
            OrderDate = order.CreatedAt,
            Status = order.Status.ToString(),
            OrderItem = new OrderItemResponse
            {
                Id = order.Item.Id,
                ItemId = order.Item.ServiceId ?? order.Item.DatasetId ?? Guid.Empty,
                Type = itemRequest.Type,
                PriceSnapshot = order.Item.PriceSnapshot
            }
        };

        _logger.LogInformation("Order {OrderId} updated successfully for client {ClientId}", order.Id, clientId);

        return _responseHandler.Success(orderResponse, "Order updated successfully");
    }

    public async Task<Response<OrderResponse>> UpdateOrderStatusAsync(
    string userId,
    string role,
    Guid orderId,
    OrderStatus newStatus,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("UpdateOrderStatusAsync: Invalid userId");
            return _responseHandler.BadRequest<OrderResponse>("Invalid user ID");
        }

        _logger.LogInformation("Updating status of order {OrderId} to {NewStatus} by {Role} {UserId}",
            orderId, newStatus, role, userId);

        // Get order with item included
        var order = await _context.Orders
            .Include(o => o.Item)
            .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted, cancellationToken);

        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", orderId);
            return _responseHandler.NotFound<OrderResponse>("Order not found");
        }

        // Role-based authorization
        if (role.Equals("ServiceProvider", StringComparison.OrdinalIgnoreCase))
        {
            if (order.Item.Service?.ProviderId != userId && order.Item.Dataset?.ProviderId != userId)
            {
                _logger.LogWarning("Unauthorized attempt to update order {OrderId} by provider {UserId}", orderId, userId);
                return _responseHandler.Unauthorized<OrderResponse>("You are not authorized to update this order");
            }
        }
        else if (role.Equals("Client", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Client {UserId} attempted to update order {OrderId} status", userId, orderId);
            return _responseHandler.Unauthorized<OrderResponse>("Clients cannot change order status");
        }
        else if (!role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            return _responseHandler.Unauthorized<OrderResponse>("Invalid role");
        }

        // Update status
        var oldStatus = order.Status;
        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        // Create audit log entry
        _context.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityId = order.Id,
            EntityName = nameof(Entities.Models.Order),
            Action = "Update Status",
            Details = $"Status changed from {oldStatus} to {newStatus}",
            UserId = userId,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        //Ziad : TODO : Send notification to client


        var orderResponse = new OrderResponse
        {
            Id = order.Id,
            ClientId = order.ClientId,
            Amount = order.Amount,
            Commission = order.Commission,
            OrderDate = order.CreatedAt,
            Status = order.Status.ToString(),
            OrderItem = new OrderItemResponse
            {
                Id = order.Item.Id,
                ItemId = order.Item.ServiceId ?? order.Item.DatasetId ?? Guid.Empty,
                PriceSnapshot = order.Item.PriceSnapshot
            }
        };

        _logger.LogInformation("Order {OrderId} status updated from {OldStatus} to {NewStatus} by {Role} {UserId}",
            order.Id, oldStatus, newStatus, role, userId);

        return _responseHandler.Success(orderResponse, "Order status updated successfully");
    }

    public async Task<Response<PaginatedList<OrderResponse>>> GetAllOrdersForAdminAsync(AdminOrderFilters<OrderSorting> filters, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => (u.Id == filters.ClientId || u.Id == filters.ProviderId), cancellationToken);

        if (user == null && (filters.ClientId != null || filters.ProviderId != null))
        {
            _logger.LogWarning("GetAllOrdersForAdminAsync: Invalid UserId");
            return _responseHandler.BadRequest<PaginatedList<OrderResponse>>("Invalid User Id");
        }

        _logger.LogInformation("Fetching orders for user {UserId} with filters: {@Filters}", user!.Id, filters);

        var query = _context.Orders
        .Where(o => o.ClientId == user.Id && !o.IsDeleted);

        var filteredList = FilterForAdmin(query, filters, user.Id);

        var source = filteredList.Include(o => o.Item)
                .Select(o => new OrderResponse
                {
                    Id = o.Id,
                    Amount = o.Amount,
                    Commission = o.Commission,
                    Status = o.Status.ToString(),
                    OrderItem = new OrderItemResponse
                    {
                        ItemId = o.Item.ServiceId != null && o.Item.ServiceId != Guid.Empty
                                                                                ? o.Item.ServiceId
                                                                                : o.Item.DatasetId,

                        PriceSnapshot = o.Item.PriceSnapshot,
                        Id = o.Item.Id,
                    }
                })
                .AsNoTracking().AsQueryable();


        var orders = await PaginatedList<OrderResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

        _logger.LogInformation("Fetched {Count} orders for user {User} on page {PageNumber} with page size {PageSize}",
            orders.Items.Count, user.Id, filters.PageNumber, filters.PageSize);

        return _responseHandler.Success(orders, "Orders fetched successfully");
    }



    public async Task<Response<string>> DeleteOrderAsync(string clientId, Guid orderId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(clientId))
        {
            _logger.LogWarning("DeleteOrderAsync: Invalid clientId");
            return _responseHandler.BadRequest<string>("Invalid client ID");
        }

        _logger.LogInformation("Deleting order {OrderId} for client {ClientId}", orderId, clientId);

        var order = await _context.Orders
            .Include(o => o.Item)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.ClientId == clientId, cancellationToken);

        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found for client {ClientId}", orderId, clientId);
            return _responseHandler.NotFound<string>("Order not found");
        }

        if (order.Status != OrderStatus.PendingPayment)
        {
            _logger.LogWarning("Cannot delete order {OrderId} because status is {Status}", orderId, order.Status);
            return _responseHandler.BadRequest<string>("Only orders in PendingPayment status can be deleted");
        }

        order.IsDeleted = true;
        order.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} deleted successfully for client {ClientId}", orderId, clientId);

        return _responseHandler.Success<string>(null, "Order deleted successfully");
    }


    #region request a service for client
    public async Task<Response<OrderResponse>> RequestServiceAsync(string clientId, Guid ServiceId, Guid DataSetId, RequestServiceDto request)
    {
        try
        {

            var service = await _context.Services
                .Include(s => s.Provider)
                .FirstOrDefaultAsync(s => s.Id == ServiceId && !s.IsDeleted);

            if (service == null)
            {
                _logger.LogWarning($"No service with serviceId : {ServiceId}");
                return _responseHandler.NotFound<OrderResponse>("Service not found.");
            }

            var order = new Entities.Models.Order
            {
                Id = Guid.NewGuid(),
                ClientId = clientId,
                Status = OrderStatus.PendingPayment,
                Amount = request.Quantity,
                Commission = request.Budget,
                Item = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ServiceId = service.Id,
                    PriceSnapshot = service.Price,
                    ApiKey = request.ApiKey,
                    DownloadLink = request.DownloadUrl,
                    DatasetId = DataSetId
                }
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Order Requested Successfull with orderId :{OrderId} \n and orderItem with ortemItemId : {OrtemItemId}", order.Id, order.Item.Id);
            var responseD = new OrderResponse
            {
                Id = order.Id,
                Amount = order.Amount,
                Commission = order.Commission,
                Status = order.Status.ToString(),
                OrderItem = new OrderItemResponse
                {
                    Id = order.Item.Id,
                    ItemId = string.IsNullOrEmpty(order.Item.ServiceId.ToString()) ? order.Item.ServiceId : order.Item.DatasetId,
                    PriceSnapshot = order.Item.PriceSnapshot
                }

            };

            return _responseHandler.Created(responseD, "Service request created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating service request for Client: {ClientId}", clientId);
            return _responseHandler.ServerError<OrderResponse>("An error occurred while creating the service request.");
        }
    }

    #endregion


    private IQueryable<Entities.Models.Order> FilterForUser<TSorting>(IQueryable<Entities.Models.Order> query, OrderFilters<TSorting> filters, string? clientId)
          where TSorting : struct, Enum
    {
        if (filters.SortColumn is not null)
        {
            query = ApplySorting(query, filters.SortColumn.Value, filters.SortDirection);
            _logger.LogInformation("Sorting orders for user {UserId} by {SortProperty} in {SortDirection} order",
                clientId, filters.SortColumn, filters.SortDirection);
        }
        else
        {
            query = query.OrderByDescending(o => o.CreatedAt);
        }
        if (filters.Status is not null)
        {
            query = query.Where(o => o.Status == filters.Status);
            _logger.LogInformation("Filtered orders for user {UserId} by status {Status} ",
                clientId, filters.Status);
        }

        return query;
    }
    private IQueryable<Entities.Models.Order> FilterForAdmin<TSorting>(IQueryable<Entities.Models.Order> query, AdminOrderFilters<TSorting> filters, string? clientId)
                  where TSorting : struct, Enum
    {
        if (filters.SortColumn is not null)
        {
            query = ApplySorting(query, filters.SortColumn.Value, filters.SortDirection);
            _logger.LogInformation("Sorting orders for user {UserId} by {SortProperty} in {SortDirection} order",
                clientId, filters.SortColumn, filters.SortDirection);
        }
        else
        {
            query = query.OrderByDescending(o => o.CreatedAt);
        }
        if (filters.Status is not null)
        {
            query = query.Where(o => o.Status == filters.Status);
            _logger.LogInformation("Filtered orders for user {UserId} by status {Status} ",
                clientId, filters.Status);
        }
        return query;
    }

    private static IQueryable<Entities.Models.Order> ApplySorting<TSorting>(IQueryable<Entities.Models.Order> query, TSorting sortColumn, SortDirection? direction)
    {
        var isAscending = direction == SortDirection.ASC;

        return sortColumn switch
        {
            OrderSorting.OrderDate => isAscending ? query.OrderBy(o => o.CreatedAt) : query.OrderByDescending(o => o.CreatedAt),
            OrderSorting.Status => isAscending ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status),
            _ => query.OrderByDescending(o => o.CreatedAt)
        };
    }

}
