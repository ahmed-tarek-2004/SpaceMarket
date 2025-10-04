using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.Models
{
    public class ServiceMetricEvent
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public string? UserId { get; set; } // if user is guest or is not registered
        public ServiceEventType EventType { get; set; } // enum: View, Click, Request
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        //public string? MetadataJson { get; set; }
        public Service Service { get; set; }
    }
}
