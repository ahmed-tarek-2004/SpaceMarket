using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.DebrisTracking
{
    public class PositionDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double AltitudeKm { get; set; }
    }
}
