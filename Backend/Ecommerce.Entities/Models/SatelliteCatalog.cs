using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.Models
{
    public class SatelliteCatalog
    {
        public Guid Id { get; set; }
        public string NoradId { get; set; }      // ex: "25544"
        public string Name { get; set; }         // ex: "ISS (ZARYA)"
        public string TleLine1 { get; set; }
        public string TleLine2 { get; set; }

        public DateTime LastSyncedAt { get; set; } = DateTime.UtcNow;
    }

}
