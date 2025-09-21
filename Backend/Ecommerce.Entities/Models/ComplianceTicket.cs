using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Users;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.Models
{
    public class ComplianceTicket
    {
        public Guid Id { get; set; }
        public string ClientId { get; set; }
        public string? ProviderId { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string AttachmentsUrl { get; set; }
        public DateTime PreferredDate { get; set; }
        public ComplianceTicketStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Client Client { get; set; }
        public ServiceProvider Provider { get; set; }
        public ICollection<ComplianceMessage> Messages { get; set; }
    }
}
