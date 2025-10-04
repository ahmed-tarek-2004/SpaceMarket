using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Payment
{
    public class HandlePayment
    {
       public Guid OrderId {  get; set; }
       public string SessionId {  get; set; }
    }
}
