using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Users;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public decimal Amount { get; set; }
        public decimal Commission { get; set; }
        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Client Client { get; set; }
        public ICollection<OrderItem> Items { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
