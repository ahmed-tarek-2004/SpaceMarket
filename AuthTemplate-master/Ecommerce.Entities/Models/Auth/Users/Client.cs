using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Identity;

namespace Ecommerce.Entities.Models.Auth.Users
{
    public class Client
    {
        public string Id { get; set; } // PK, FK → User.Id
        public string FullName { get; set; }
        public string Organization { get; set; }
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public User User { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    }
}
