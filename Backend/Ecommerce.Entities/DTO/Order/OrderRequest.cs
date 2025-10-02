
namespace Ecommerce.Entities.DTO.Order;
public class OrderRequest
{
    public string ClientId { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public OrderItemRequest OrderItem { get; set; }
    public decimal Commission { get; set; }

    public decimal TotalPrice => OrderItem.PriceSnapshot + Commission;
}