using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.ServiceCatalog
{
    public class ServiceListFilterRequest
    {
        public Guid? CategoryId { get; set; }
        public string? ProviderId { get; set; }
        public string? Status { get; set; } // Active, PendingApproval, Suspended
    }

}
