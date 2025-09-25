using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Entities.DTO.ServiceCatalog
{
    public class UpdateServiceRequest
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal? Price { get; set; }
        public IFormFile? Image { get; set; }
    }
}
