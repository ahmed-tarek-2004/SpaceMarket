using Ecommerce.Utilities.Enums;
using System.Text.Json.Serialization;

namespace Ecommerce.Entities.DTO.Shared;

public class AdminOrderFilters<TSortColumn> : RequestFilters<TSortColumn>
    where TSortColumn : struct, Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus? Status { get; set; }
    public string? ClientId { get; set; }
    public string? ProviderId { get; set; }
}
