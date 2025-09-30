using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.DTO.Order;
public class OrderResponse
{
    public Guid Id { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Commission { get; set; }
    public decimal TotalPrice => OrderItem.PriceSnapshot + Commission;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = OrderStatus.PendingPayment.ToString();
    public OrderItemResponse OrderItem { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string DownloadLink { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}