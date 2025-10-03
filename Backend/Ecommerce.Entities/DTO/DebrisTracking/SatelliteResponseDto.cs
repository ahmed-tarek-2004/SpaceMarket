using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.DebrisTracking
{
    public class SatelliteResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NoradId { get; set; }
        public double ProximityThresholdKm { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
