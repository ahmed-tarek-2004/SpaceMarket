using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.Models
{
    public class CommissionSetting
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public decimal RatePercent { get; set; }

        public ServiceCategory Category { get; set; }
    }
}
