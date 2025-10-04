using System;
using Ecommerce.Entities.DTO.DebrisTracking;
using SGPdotNET.CoordinateSystem;
using SGPdotNET.Propagation;
using SGPdotNET.TLE;

namespace Ecommerce.DataAccess.Services.OrbitalPropagation
{
    public class OrbitalPropagationService : IOrbitalPropagationService
    {
        public PositionDto PropagateToGeodetic(string tleLine1, string tleLine2, DateTime epochUtc)
        {
            // 1) Parse TLE
            var tle = new Tle(tleLine1, tleLine2);

            // 2) Create propagator
            var propagator = new Sgp4(tle);

            // 3) Propagate to ECI position
            EciCoordinate eci = propagator.FindPosition(epochUtc);

            // 4) Convert to Geodetic (Lat, Lon, Alt)
            GeodeticCoordinate geo = eci.ToGeodetic();

            // 5) Map to your DTO
            return new PositionDto
            {
                Latitude = geo.Latitude.Degrees,   // already in degrees
                Longitude = geo.Longitude.Degrees, // already in degrees
                AltitudeKm = geo.Altitude  // km
            };
        }
    }
}
