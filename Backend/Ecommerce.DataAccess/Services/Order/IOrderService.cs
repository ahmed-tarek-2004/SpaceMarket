using Ecommerce.Entities.DTO.Order;
using Ecommerce.Entities.DTO.Shared;
using Ecommerce.Entities.Shared;
using Ecommerce.Entities.Shared.Bases;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.DataAccess.Services.Order;
public interface IOrderService
{
    Task<Response<PaginatedList<OrderResponse>>> GetAllOrdersAsync(string clientId, OrderFilters<OrderSorting> filters, CancellationToken cancellationToken = default);
    Task<Response<OrderResponse>> GetOrderByIdAsync(string clientId, Guid orderId, CancellationToken cancellationToken = default);
}
