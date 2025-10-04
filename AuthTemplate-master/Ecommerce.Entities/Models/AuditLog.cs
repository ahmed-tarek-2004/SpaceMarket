using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Identity;

namespace Ecommerce.Entities.Models
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public string UserId { get; set; }
        public string EntityName { get; set; }
        public Guid EntityId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Details { get; set; }

        public User User { get; set; }
    }
}
