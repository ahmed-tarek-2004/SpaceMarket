using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Utilities.Enums;
namespace Ecommerce.Entities.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public Order Order { get; set; }
    }

}
