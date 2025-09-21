using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Users;

namespace Ecommerce.Entities.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public Guid ClientId { get; set; }
        public Guid ProviderId { get; set; }
        public int Rating { get; set; } // 1–5
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Service Service { get; set; }
        public Client Client { get; set; }
        public ServiceProvider Provider { get; set; }
        public ReviewResponse Response { get; set; }
    }
}
