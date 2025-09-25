using Ecommerce.Utilities.Enums;
using System.Text.Json.Serialization;

namespace Ecommerce.Entities.DTO.Shared;
public class RequestFilters<TSortColumn>
    where TSortColumn : struct, Enum
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    //public string? SearchValue { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TSortColumn? SortColumn { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortDirection? SortDirection { get; init; } = Utilities.Enums.SortDirection.ASC;
}


