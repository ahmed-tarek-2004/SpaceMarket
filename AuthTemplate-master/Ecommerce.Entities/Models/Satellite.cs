using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Users;

namespace Ecommerce.Entities.Models
{
    public class Satellite
    {
        public Guid Id { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string NoradId { get; set; }
        public string TleLine1 { get; set; }
        public string TleLine2 { get; set; }
        public double ProximityThresholdKm { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Client Client { get; set; }
        public ICollection<CollisionAlert> CollisionAlerts { get; set; }
    }
}
