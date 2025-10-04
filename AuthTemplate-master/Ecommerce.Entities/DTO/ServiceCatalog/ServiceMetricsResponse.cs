using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.ServiceCatalog
{
    public class ServiceMetricsResponse
    {
        public Guid ServiceId { get; set; }
        public string Title { get; set; }
        public int ViewsCount { get; set; }
        public int ClicksCount { get; set; }
        public int RequestsCount { get; set; }
    }

}
