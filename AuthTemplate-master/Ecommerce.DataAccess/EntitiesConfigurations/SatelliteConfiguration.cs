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
    public class SatelliteConfiguration : IEntityTypeConfiguration<Satellite>
    {
        public void Configure(EntityTypeBuilder<Satellite> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.ClientId).IsRequired().HasMaxLength(450);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
            builder.Property(s => s.NoradId).IsRequired().HasMaxLength(50);
            builder.Property(s => s.TleLine1).IsRequired().HasMaxLength(100);
            builder.Property(s => s.TleLine2).IsRequired().HasMaxLength(100);
            builder.Property(s => s.ProximityThresholdKm).IsRequired();

            builder.Property(s => s.CreatedAt).IsRequired();
            builder.Property(s => s.UpdatedAt);

            // Many-to-One with Client
            builder.HasOne(s => s.Client)
                .WithMany()
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with CollisionAlerts
            builder.HasMany(s => s.CollisionAlerts)
                .WithOne(ca => ca.Satellite)
                .HasForeignKey(ca => ca.SatelliteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
