namespace Ecommerce.Entities.DTO.Order;
public class OrderResponse
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public decimal Commission { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemResponse>? OrderItems { get; set; }
}
