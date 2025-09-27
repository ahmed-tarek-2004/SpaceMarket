using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Payment
{
    public class PaymentRequest
    {
        public string ServiceName { get; set; }
        public decimal ServiceUnitAmount { get; set; }    
        public int Quantity { get; set; }            
        public string Currency { get; set; } = "usd";
        public string SuccessUrl { get; set; } = "https://amars-marvelous-site-305200.webflow.io/";
        public string CancelUrl { get; set; } = "https://amars-fantabulous-site-16cb2e.webflow.io/";
    }
}
