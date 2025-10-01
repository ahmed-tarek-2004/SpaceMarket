using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.ServiceCatalog
{
    public class DatasetResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal Price { get; set; }
        public string? FileUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? ApiEndpoint { get; set; }
        public string ProviderId { get; set; } = null!;
        public string ProviderName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
