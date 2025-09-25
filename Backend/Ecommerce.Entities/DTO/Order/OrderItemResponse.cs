namespace Ecommerce.Entities.DTO.Order;
public class OrderItemResponse
{
    public Guid Id { get; set; }
    public Guid DatasetId { get; set; }
    public int Quantity { get; set; }
    public decimal PriceSnapshot { get; set; }

}
