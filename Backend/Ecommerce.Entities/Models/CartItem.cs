using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Users;

namespace Ecommerce.Entities.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }

        public Guid? CartId { get; set; }
        public Cart Cart { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ClientId { get; set; }
        public Client Client { get; set; }

        public Guid? ServiceId { get; set; }
        public Service Service { get; set; }

        public Guid? DatasetId { get; set; }
        public Dataset Dataset { get; set; }

        public int Quantity { get; set; } = 1;
        public decimal PriceSnapshot { get; set; }

        public Guid? OrderItemId { get; set; }
        public OrderItem Item { get; set; }
    }

}
