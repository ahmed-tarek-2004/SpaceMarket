using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Identity;

namespace Ecommerce.Entities.Models
{
    public class ComplianceMessage
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public string SenderId { get; set; }
        public string Text { get; set; }
        public string AttachmentUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ComplianceTicket Ticket { get; set; }
        public User Sender { get; set; }
    }
}
