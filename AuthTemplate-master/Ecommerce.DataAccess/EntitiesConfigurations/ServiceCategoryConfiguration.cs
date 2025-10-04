using Ecommerce.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
    {
        public void Configure(EntityTypeBuilder<ServiceCategory> builder)
        {
            builder.HasKey(sc => sc.Id);

            builder.Property(sc => sc.Name).IsRequired().HasMaxLength(100);
            builder.Property(sc => sc.Description).HasMaxLength(500);

            builder.Property(sc => sc.IsDeleted).HasDefaultValue(false);

            builder.HasMany(sc => sc.Services)
                .WithOne(s => s.Category)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if services exist

            builder.Property(sc => sc.CreatedAt).IsRequired();
            builder.Property(sc => sc.UpdatedAt);
        }
    }

}
