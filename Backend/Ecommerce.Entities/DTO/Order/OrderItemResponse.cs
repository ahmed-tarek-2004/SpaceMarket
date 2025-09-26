namespace Ecommerce.Entities.DTO.Order;
public class OrderItemResponse
{
    public Guid Id { get; set; }
    public Guid DatasetId { get; set; }
    public int Quantity { get; set; }

    public string?ServiceId { get; set; }
    //public string DataSetId { get; set; }
    public decimal PriceSnapshot { get; set; }

}
