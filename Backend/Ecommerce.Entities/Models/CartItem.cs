using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ServiceId { get; set; }
        public Guid DatasetId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal PriceSnapshot { get; set; }

        public Cart Cart { get; set; }
        public Service Service { get; set; }
        public Dataset Dataset { get; set; }
    }
}
