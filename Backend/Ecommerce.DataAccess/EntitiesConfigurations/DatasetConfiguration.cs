using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class DatasetConfiguration : IEntityTypeConfiguration<Dataset>
    {
        public void Configure(EntityTypeBuilder<Dataset> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Title).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Description).HasMaxLength(500);
            builder.Property(d => d.FileUrl).HasMaxLength(500);
            builder.Property(d => d.ApiEndpoint).HasMaxLength(500);
            builder.Property(d => d.Price).HasColumnType("decimal(18,2)");
            //builder.Property(d => d.MetadataJson).HasMaxLength(2000);
            builder.Property(d => d.ThumbnailUrl).HasMaxLength(500);

            // Enum as string with max length
            builder.Property(d => d.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(d => d.CreatedAt).IsRequired();
            builder.Property(d => d.UpdatedAt);

            // Soft delete properties
            builder.Property(d => d.IsDeleted).HasDefaultValue(false);
            builder.HasQueryFilter(d => !d.IsDeleted);

            // Many-to-One with ServiceProvider
            builder.HasOne(d => d.Provider)
                .WithMany(sp => sp.Datasets)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-One with ServiceCategory
            builder.HasOne(d => d.Category)
                .WithMany(c => c.Datasets)   // لازم تضيف ICollection<Dataset> Datasets في ServiceCategory
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
