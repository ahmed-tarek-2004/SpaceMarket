using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.DTO.DebrisTracking
{
    public class RegisterSatelliteRequest
    {
        public Guid CatalogSatelliteId { get; set; } // select from SatelliteCatalog
        public string Name { get; set; } // override or copy from catalog
        public double ProximityThresholdKm { get; set; }
    }
}
