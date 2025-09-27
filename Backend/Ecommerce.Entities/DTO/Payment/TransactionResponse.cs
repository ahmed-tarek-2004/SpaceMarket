using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.DTO.Payment
{
    public class TransactionResponse
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
