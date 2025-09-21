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
        public double Altitude { get; set; }
        public double Velocity { get; set; }
        public DateTime LastFetchedAt { get; set; }
    }
}
