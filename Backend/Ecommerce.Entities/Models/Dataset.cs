using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models.Auth.Users;
using Ecommerce.Utilities.Enums;

namespace Ecommerce.Entities.Models
{
    public class Dataset
    {
        public Guid Id { get; set; }
        public string ProviderId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string FileUrl { get; set; }
        public string ApiEndpoint { get; set; }
        public decimal Price { get; set; }
        public string MetadataJson { get; set; }
        public string ThumbnailUrl { get; set; }
        public ServiceStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ServiceProvider Provider { get; set; }
    }

}