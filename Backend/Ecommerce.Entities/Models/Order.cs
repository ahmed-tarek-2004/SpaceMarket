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
        public decimal Amount { get; set; }
        public decimal Commission { get; set; }
        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string ClientId { get; set; }
        public Guid OrderItemId { get; set; }
<<<<<<< HEAD
        public Guid? TransactionId { get; set; }

=======
>>>>>>> 235d01283defbaea892373db620500bcfd2befe0
        public Client Client { get; set; }
        public OrderItem Item { get; set; }  
        public Transaction Transaction { get; set; }
    }
}
