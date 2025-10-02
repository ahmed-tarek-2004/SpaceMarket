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
        public string ItemType { get; set; }     
        public Guid? ItemId { get; set; }
        public string Title { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }     
        public decimal? CommissionPercent { get; set; }
        public decimal? CommissionAmount { get; set; }
        public decimal ProviderAmount { get; set; }
        public string? ProviderName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
