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
    public class CollisionAlertConfiguration : IEntityTypeConfiguration<CollisionAlert>
    {
        public void Configure(EntityTypeBuilder<CollisionAlert> builder)
        {
            builder.HasKey(ca => ca.Id);

            builder.Property(ca => ca.ClosestDistanceKm).IsRequired();
            builder.Property(ca => ca.Timestamp).IsRequired();

            // Enum as string with max length
            builder.Property(ca => ca.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Many-to-One with Satellite
            builder.HasOne(ca => ca.Satellite)
                .WithMany(s => s.CollisionAlerts)
                .HasForeignKey(ca => ca.SatelliteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-One with Debris
            builder.HasOne(ca => ca.Debris)
                .WithMany()
                .HasForeignKey(ca => ca.DebrisId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
