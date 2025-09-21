using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.Models
{
    public class CollisionAlert
    {
        public Guid Id { get; set; }
        public Guid SatelliteId { get; set; }
        public Guid DebrisId { get; set; }
        public double ClosestDistanceKm { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public CollisionAlertStatus Status { get; set; }

        public Satellite Satellite { get; set; }
        public Debris Debris { get; set; }
    }
}
