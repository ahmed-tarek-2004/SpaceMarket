using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.ServiceCatalog
{
    public class ServiceFilterResponse
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ProviderName { get; set; }
        public decimal Price { get; set; }
        public string ImagesUrl { get; set; }
        public string WebsiteUrl { get; set; }
    }
}