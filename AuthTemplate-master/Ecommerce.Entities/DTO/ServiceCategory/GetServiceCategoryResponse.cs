namespace Ecommerce.Entities.DTO.ServiceCategory
{
    public class GetServiceCategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
