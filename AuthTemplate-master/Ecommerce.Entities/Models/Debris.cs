using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entities.Models
{
    public class Debris
    {
        public Guid Id { get; set; }
        public string NoradId { get; set; }
        public string Name { get; set; }

        public string TleLine1 { get; set; }
        public string TleLine2 { get; set; }

        public DateTime LastFetchedAt { get; set; } // when we last got the Tles from Celestrak/Space-Track
    }
}
