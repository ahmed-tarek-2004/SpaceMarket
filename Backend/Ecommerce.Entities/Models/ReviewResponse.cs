using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Users;

namespace Ecommerce.Entities.Models
{
    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public Guid ReviewId { get; set; }
        public Guid ProviderId { get; set; }
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Review Review { get; set; }
        public ServiceProvider Provider { get; set; }
    }
}
