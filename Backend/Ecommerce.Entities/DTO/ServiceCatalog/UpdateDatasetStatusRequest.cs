using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.ServiceCatalog
{
    public class UpdateDatasetStatusRequest
    {
        public Guid DatasetId { get; set; }
        public string Status { get; set; } = null!;
        public string? Reason { get; set; }
    }

}
