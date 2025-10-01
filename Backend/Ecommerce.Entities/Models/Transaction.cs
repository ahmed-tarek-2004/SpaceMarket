using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Users;
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
        public string ClientId { get; set; }
        public string ServiceProviderId { get; set; }
        public Order Order { get; set; }
        public Client Client { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
    }

}
