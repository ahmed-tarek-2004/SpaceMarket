using Ecommerce.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Title).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Description).HasMaxLength(500);
            builder.Property(s => s.Price).HasColumnType("decimal(18,2)");
            builder.Property(s => s.ImagesUrlJson).HasMaxLength(1000);

            // Enum as string with max length
            builder.Property(s => s.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(s => s.CreatedAt).IsRequired();
            builder.Property(s => s.UpdatedAt);

            // Soft delete properties
            builder.Property(s => s.IsDeleted).HasDefaultValue(false);
            builder.HasQueryFilter(s => !s.IsDeleted);

            // Many-to-One with ServiceProvider
            builder.HasOne(s => s.Provider)
                .WithMany(sp => sp.Services)
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-One with ServiceCategory
            builder.HasOne(s => s.Category)
                .WithMany()
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete to preserve service history
        }
    }
}
