using Ecommerce.Entities.DTO.Order;
using Ecommerce.Entities.DTO.Shared;
using Ecommerce.Entities.Shared;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.DataAccess.Services.Order;
public interface IOrderService
{
    Task<Response<PaginatedList<OrderResponse>>> GetAllOrdersAsync(string userId, string role, OrderFilters<OrderSorting> filters, CancellationToken cancellationToken = default);
    Task<Response<OrderResponse>> GetOrderByIdAsync(string clientId, Guid orderId, CancellationToken cancellationToken = default);
    Task<Response<PaginatedList<OrderResponse>>> GetAllOrdersForAdminAsync(AdminOrderFilters<OrderSorting> filters, CancellationToken cancellationToken);
    Task<Response<OrderResponse>> CreateOrderAsync(string clientId, OrderRequest request, CancellationToken cancellationToken = default);
    Task<Response<OrderResponse>> UpdateOrderAsync(string clientId, UpdateOrderRequest request, CancellationToken cancellationToken = default);
    Task<Response<OrderResponse>> UpdateOrderStatusAsync(string userId, string role, Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken);
    Task<Response<string>> DeleteOrderAsync(string clientId, Guid orderId, CancellationToken cancellationToken = default);
    public Task<Response<OrderResponse>> RequestServiceAsync(string clientId, Guid ServiceId, Guid DataSetId, RequestServiceDto request);
}
