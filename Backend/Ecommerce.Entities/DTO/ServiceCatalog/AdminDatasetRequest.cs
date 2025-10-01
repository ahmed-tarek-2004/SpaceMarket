using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.ServiceCatalog
{
    public class AdminDatasetResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string ProviderId { get; set; } = null!;
        public string ProviderName { get; set; } = null!;
        public string ProviderEmail { get; set; } = null!;
        public decimal Price { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string CategoryName { get; set; }

    }
}
