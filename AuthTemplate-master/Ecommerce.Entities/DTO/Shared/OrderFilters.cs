using Ecommerce.Utilities.Enums;
using System.Text.Json.Serialization;

namespace Ecommerce.Entities.DTO.Shared;

public class OrderFilters<TSortColumn> : RequestFilters<TSortColumn>
    where TSortColumn : struct, Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus? Status { get; set; }
}
