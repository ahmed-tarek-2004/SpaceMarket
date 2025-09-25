using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.ServiceCatalog
{
    public class AdminServiceResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string ProviderEmail { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
