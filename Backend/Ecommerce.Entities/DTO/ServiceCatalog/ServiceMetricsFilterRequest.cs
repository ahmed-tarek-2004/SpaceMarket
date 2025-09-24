using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.ServiceCatalog
{
    public class ServiceMetricsFilterRequest
    {
        public DateTime? StartDate { get; set; } // لو ماجاش نعتبر آخر 30 يوم
        public DateTime? EndDate { get; set; }   // لو ماجاش نعتبر اليوم الحالي
    }

}
