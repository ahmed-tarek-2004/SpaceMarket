using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal PriceSnapshot { get; set; }
        public string DownloadLink { get; set; }
        public string ApiKey { get; set; }
        public Guid OrderId { get; set; }
        public Guid DatasetId { get; set; }
        public Guid ServiceId { get; set; }
        public Order Order { get; set; }
        public Dataset Dataset { get; set; }
        public Service Service { get; set; }
    }

}
