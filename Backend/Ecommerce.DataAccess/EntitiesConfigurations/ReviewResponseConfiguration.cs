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
    public class ReviewResponseConfiguration : IEntityTypeConfiguration<ReviewResponse>
    {
        public void Configure(EntityTypeBuilder<ReviewResponse> builder)
        {
            builder.HasKey(rr => rr.Id);

            builder.Property(rr => rr.Text).HasMaxLength(1000);

            builder.Property(rr => rr.CreatedAt).IsRequired();
            builder.Property(rr => rr.UpdatedAt);

            // Many-to-One with ServiceProvider
            builder.HasOne(rr => rr.Provider)
                .WithMany()
                .HasForeignKey(rr => rr.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
