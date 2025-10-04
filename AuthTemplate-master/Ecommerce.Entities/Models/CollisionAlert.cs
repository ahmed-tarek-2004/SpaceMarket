using System;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.Models
{
    public class CollisionAlert
    {
        public Guid Id { get; set; }
        public Guid SatelliteId { get; set; }
        public Guid DebrisId { get; set; }

        // Closest distance at event
        public double ClosestDistanceKm { get; set; }

        // Satellite position snapshot
        public double SatLatitude { get; set; }
        public double SatLongitude { get; set; }
        public double SatAltitudeKm { get; set; }

        // Debris position snapshot
        public double DebrisLatitude { get; set; }
        public double DebrisLongitude { get; set; }
        public double DebrisAltitudeKm { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public CollisionAlertStatus Status { get; set; }

        public Satellite Satellite { get; set; }
        public Debris Debris { get; set; }
    }
}
