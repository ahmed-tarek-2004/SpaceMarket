using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.DebrisTracking
{
    public class CollisionAlertResponse
    {
        public Guid SatelliteId { get; set; }
        public string SatelliteName { get; set; }
        public PositionDto SatellitePosition { get; set; }

        public Guid DebrisId { get; set; }
        public string DebrisName { get; set; }
        public PositionDto DebrisPosition { get; set; }

        public double ClosestDistanceKm { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
    }
}
