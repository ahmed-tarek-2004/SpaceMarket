using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.DTO.DebrisTracking;

namespace Ecommerce.DataAccess.Services.OrbitalPropagation
{
    public interface IOrbitalPropagationService
    {
        /// <summary>
        /// Given TLE lines and target epoch, returns geodetic position (lat, lon, alt in km).
        /// </summary>
        PositionDto PropagateToGeodetic(string tleLine1, string tleLine2, DateTime epochUtc);
    }
}
