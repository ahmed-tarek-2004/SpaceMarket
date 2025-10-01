using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.ServiceCatalog
{
    public class DatasetFilterResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public string ProviderName { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
