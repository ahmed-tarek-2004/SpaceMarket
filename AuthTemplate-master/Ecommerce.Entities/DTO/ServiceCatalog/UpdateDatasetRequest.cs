using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Entities.DTO.ServiceCatalog
{
    public class UpdateDatasetRequest
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal? Price { get; set; }
        public IFormFile? File { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public string? ApiEndpoint { get; set; }
    }
}
