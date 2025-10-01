namespace Ecommerce.Entities.Models
{
    public class ServiceCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public List<Service> Services { get; set; } = new List<Service>();
        public List<Dataset> Datasets { get; set; } = new List<Dataset>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
