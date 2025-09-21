using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Users;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid ServiceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ProgressPercent { get; set; }
        public ProjectStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Client Client { get; set; }
        public Service Service { get; set; }
    }
}
