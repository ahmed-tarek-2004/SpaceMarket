using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.DTO.Order;
public class OrderItemRequest
{
    public Guid ItemId { get; set; }
    public ItemType Type { get; set; }
    public decimal PriceSnapshot { get; set; }

}