using Ecommerce.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Payment
{
    public class ProviderGetOrdersDto
    {
         public Guid OrderId { get; set; }
         public string ClientName { get; set; }
         public decimal Amount { get; set; }
         public OrderStatus PaymentStatus { get; set; } 
         public DateTime CreatedAt { get; set; }
    }
}
