using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class ComplianceServiceConfiguration : IEntityTypeConfiguration<ComplianceService>
    {
        public void Configure(EntityTypeBuilder<ComplianceService> builder)
        {
            builder.HasKey(cs => cs.Id);

            builder.Property(cs => cs.Title).IsRequired().HasMaxLength(100);
            builder.Property(cs => cs.Description).HasMaxLength(500);
            builder.Property(cs => cs.Category).HasMaxLength(100);
            builder.Property(cs => cs.CertificationsUrl).HasMaxLength(500);
            builder.Property(cs => cs.Price).HasColumnType("decimal(18,2)");

            builder.Property(cs => cs.CreatedAt).IsRequired();
            builder.Property(cs => cs.UpdatedAt);

            // Soft delete properties
            builder.Property(cs => cs.IsDeleted).HasDefaultValue(false);
            builder.HasQueryFilter(cs => !cs.IsDeleted);

            // Many-to-One with ServiceProvider
            builder.HasOne(cs => cs.Provider)
                .WithMany()
                .HasForeignKey(cs => cs.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
