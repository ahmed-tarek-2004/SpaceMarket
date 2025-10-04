namespace Ecommerce.Entities.DTO.Order;
public class UpdateOrderRequest
{
    public Guid OrderId { get; set; }
    public OrderItemRequest OrderItem { get; set; }
}
