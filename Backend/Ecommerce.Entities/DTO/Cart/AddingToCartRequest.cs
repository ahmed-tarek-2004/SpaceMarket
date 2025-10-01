using Ecommerce.Entities.Models;
using Ecommerce.Entities.Models.Auth.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Cart
{
    public class AddingToCartRequest
    {
        public Guid? ServiceId { get; set; }
        public Guid? DataSetId { get; set; }
    }
}
