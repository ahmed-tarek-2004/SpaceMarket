//using Ecommerce.DataAccess.ApplicationContext;
//using Ecommerce.Entities.DTO.Order;
//using Ecommerce.Entities.DTO.Shared;
//using Ecommerce.Entities.Models;
//using Ecommerce.Entities.Shared;
//using Ecommerce.Entities.Shared.Bases;
//using Ecommerce.Utilities.Enums;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;

//namespace Ecommerce.DataAccess.Services.Order;
//public class OrderService(
//        ApplicationDbContext context,
//        ILogger<OrderService> logger,
//        ResponseHandler responseHandler
//    ) : IOrderService
//{
//    private readonly ApplicationDbContext _context = context;
//    private readonly ILogger<OrderService> _logger = logger;
//    private readonly ResponseHandler _responseHandler = responseHandler;

//    public async Task<Response<PaginatedList<OrderResponse>>> GetAllOrdersAsync(string clientId, OrderFilters<OrderSorting> filters, CancellationToken cancellationToken)
//    {

//        if (clientId is null)
//        {
//            _logger.LogWarning("GetClientOrdersAsync: Invalid clientId");
//            return _responseHandler.BadRequest<PaginatedList<OrderResponse>>("Invalid client ID");
//        }


//        _logger.LogInformation("Fetching orders for client {ClientId} with filters: {@Filters}", clientId, filters);


//        var query = _context.Orders
//            .Where(o => o.ClientId == clientId!.ToString() && !o.IsDeleted);


//        var filteredList = FilteredListItems(query, filters, clientId);

//        // Ziad : TODO : Edit the order and order items models to include the necessary fields for the response
//        var source = filteredList.Include(o => o.Items)
//                .Select(o => new OrderResponse
//                {
//                    Id = o.Id,
//                    Amount = o.Amount,
//                    Commission = o.Commission,
//                    Status = o.Status.ToString(),
//                    OrderItems = o.Items.Select(oi => new OrderItemResponse
//                    {
//                        DatasetId = oi.DatasetId,
//                        Quantity = oi.Quantity,
//                        PriceSnapshot = oi.PriceSnapshot,
//                        //edit for new migration (AddingServiceToOrderItem )
//                        Id = oi.Id,
//                        ServiceId = oi.ServiceId.ToString()
//                    }).ToList()
//                })
//                .AsNoTracking().AsQueryable();


//        var orders = await PaginatedList<OrderResponse>.CreateAsync(source, filters.PageNumber, filters.PageSize, cancellationToken);

//        _logger.LogInformation("Fetched {Count} orders for client {ClientId} on page {PageNumber} with page size {PageSize}",
//            orders.Items.Count, clientId, filters.PageNumber, filters.PageSize);

//        return _responseHandler.Success(orders, "Orders fetched successfully");


//    }

//    public async Task<Response<OrderResponse>> GetOrderByIdAsync(string clientId, Guid orderId, CancellationToken cancellationToken)
//    {
//        if (clientId is null)
//        {
//            _logger.LogWarning("GetClientOrdersAsync: Invalid clientId");
//            return _responseHandler.BadRequest<OrderResponse>("Invalid client ID");
//        }


//        _logger.LogInformation("Fetching order with id {OrderId} for client {ClientId}", orderId, clientId);


//        var order = await _context.Orders
//            .FirstOrDefaultAsync(o => o.ClientId == clientId!.ToString() && o.Id == orderId && !o.IsDeleted, cancellationToken);

//        if (order is null)
//        {
//            _logger.LogWarning("Order with id {OrderId} not found for client {ClientId}", orderId, clientId);
//            return _responseHandler.NotFound<OrderResponse>("Order not found");
//        }

//        var orderResponse = new OrderResponse
//        {
//            Id = order.Id,
//            Amount = order.Amount,
//            Commission = order.Commission,
//            Status = order.Status.ToString(),
//            OrderItems = order.Items.Select(oi => new OrderItemResponse
//            {
//                DatasetId = oi.DatasetId,
//                Quantity = oi.Quantity,
//                PriceSnapshot = oi.PriceSnapshot,
//                //edit for new migration (AddingServiceToOrderItem )
//                Id = oi.Id,
//                ServiceId = oi.ServiceId.ToString(),
//            }).ToList()
//        };

//        _logger.LogInformation("Fetched order {OrderId} for client {ClientId}}",
//            orderId, clientId);

//        return _responseHandler.Success<OrderResponse>(orderResponse, "Orders fetched successfully");
//    }

//    #region request a service for client
//    public async Task<Response<OrderResponse>> RequestServiceAsync(string clientId, Guid ServiceId, Guid DataSetId, RequestServiceDto request)
//    {
//        try
//        {

//            var service = await _context.Services
//                .Include(s => s.Provider)
//                .FirstOrDefaultAsync(s => s.Id == ServiceId && !s.IsDeleted);

//            if (service == null)
//            {
//                _logger.LogWarning($"No service with serviceId : {ServiceId}");
//                return _responseHandler.NotFound<OrderResponse>("Service not found.");
//            }

//            var order = new Entities.Models.Order
//            {
//                Id = Guid.NewGuid(),
//                ClientId = clientId,
//                Status = OrderStatus.PendingPayment,
//                Amount = request.Quantity,
//                Commission = request.Budget,
//                Items = new List<OrderItem>
//                {
//                      new OrderItem
//                      {
//                          Id = Guid.NewGuid(),
//                          ServiceId = service.Id,
//                          Quantity = 1,
//                          PriceSnapshot = service.Price,
//                          ApiKey=request.ApiKey,
//                          DownloadLink=request.DownloadUrl,
//                          DatasetId=DataSetId
//                      }
//                }
//            };

//            await _context.Orders.AddAsync(order);
//            await _context.SaveChangesAsync();
//            _logger.LogInformation($"Order Requested Successfull with orderId : {order.Id} \n and orderItem with ortemItemId : {order.Items.FirstOrDefault().Id}");
//            var responseD = new OrderResponse
//            {
//                Id = order.Id,
//                Amount = order.Amount,
//                Commission = order.Commission,
//                Status = order.Status.ToString(),
//                OrderItems = order.Items.Select(oi => new OrderItemResponse
//                {
//                    Id = oi.Id,
//                    ServiceId = oi.ServiceId.ToString(),
//                    Quantity = oi.Quantity,
//                    PriceSnapshot = oi.PriceSnapshot,
//                    DatasetId = DataSetId
//                }).ToList()

//            };

//            return _responseHandler.Created(responseD, "Service request created successfully.");
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error creating service request for Client: {ClientId}", clientId);
//            return _responseHandler.ServerError<OrderResponse>("An error occurred while creating the service request.");
//        }
//    }

//    #endregion
//    private IQueryable<Entities.Models.Order> FilteredListItems<TSorting>(IQueryable<Entities.Models.Order> query, OrderFilters<TSorting> filters, string? clientId)
//          where TSorting : struct, Enum
//    {
//        if (filters.SortColumn is not null)
//        {
//            query = ApplySorting(query, filters.SortColumn.Value, filters.SortDirection);
//            _logger.LogInformation("Sorting orders for user {UserId} by {SortProperty} in {SortDirection} order",
//                clientId, filters.SortColumn, filters.SortDirection);
//        }
//        else
//        {
//            query = query.OrderByDescending(o => o.CreatedAt);
//        }
//        if (filters.Status is not null)
//        {
//            query = query.Where(o => o.Status == filters.Status);
//            _logger.LogInformation("Filtered orders for user {UserId} by status {Status} ",
//                clientId, filters.Status);
//        }
//        return query;
//    }
//    private static IQueryable<Entities.Models.Order> ApplySorting<TSorting>(IQueryable<Entities.Models.Order> query, TSorting sortColumn, SortDirection? direction)
//    {
//        var isAscending = direction == SortDirection.ASC;

//        return sortColumn switch
//        {
//            OrderSorting.OrderDate => isAscending ? query.OrderBy(o => o.CreatedAt) : query.OrderByDescending(o => o.CreatedAt),
//            OrderSorting.Status => isAscending ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status),
//            _ => query.OrderByDescending(o => o.CreatedAt)
//        };
//    }

//}
