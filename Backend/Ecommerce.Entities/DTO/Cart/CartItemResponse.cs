using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.Cart
{
    public class CartItemResponse
    {
        public Guid CartItemId { get; set; }
        public Guid ServiceId{ get; set; }
        public string ServiceTitle { get; set; }
        public string ProviderName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }
}
