using Ecommerce.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Cart
{
    public class CartResponse
    {
        public string CartId { get; set; }
        public List<CartItemResponse> CartItems { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
