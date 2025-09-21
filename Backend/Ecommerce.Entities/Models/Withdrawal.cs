using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Utilities.Enums;
using Ecommerce.Entities.Models.Auth.Users;

namespace Ecommerce.Entities.Models
{
    public class Withdrawal
    {
        public Guid Id { get; set; }
        public string ProviderId { get; set; }
        public decimal Amount { get; set; }
        public string BankAccount { get; set; }
        public WithdrawalStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ServiceProvider Provider { get; set; }
    }
}
