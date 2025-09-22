using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Utilities.Enums;
namespace Ecommerce.Entities.Models.Auth.Users
{
    public class ServiceProvider
    {
        public string Id { get; set; } // PK, FK → User.Id
        public string CompanyName { get; set; }
        public string WebsiteUrl { get; set; }
        public List<string> CertificationsUrls { get; set; }
        public ServiceProviderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public User User { get; set; }
        public ICollection<Service> Services { get; set; }
        public ICollection<Dataset> Datasets { get; set; }
        public ICollection<Withdrawal> Withdrawals { get; set; }
    }
}
