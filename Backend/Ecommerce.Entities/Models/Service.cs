using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Users;
using Ecommerce.Utilities.Enums;
using Microsoft.AspNetCore.Http;
namespace Ecommerce.Entities.Models
{
    public class Service
    {
        public Guid Id { get; set; }
        public string ProviderId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        //public Guid OrderItemId { get; set; }
        public decimal Price { get; set; }
        public string ImagesUrl { get; set; }
        public ServiceStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<CartItem> CartItems {  get; set; }
        public ICollection<OrderItem> OrderItems {  get; set; }
        
        public ServiceProvider Provider { get; set; }
        public ServiceCategory Category { get; set; }
    }
}
