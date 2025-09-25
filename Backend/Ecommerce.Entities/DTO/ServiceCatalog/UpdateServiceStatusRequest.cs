using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.ServiceCatalog
{
    public class UpdateServiceStatusRequest
    {
        public Guid ServiceId { get; set; }
        public string Status { get; set; } 
        public string? Reason { get; set; } 
    }

}
