using Ecommerce.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Data.Configurations
{
    public class SatelliteCatalogConfiguration : IEntityTypeConfiguration<SatelliteCatalog>
    {
        public void Configure(EntityTypeBuilder<SatelliteCatalog> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.NoradId)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(s => s.TleLine1)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(s => s.TleLine2)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(s => s.LastSyncedAt)
                   .HasDefaultValueSql("getutcdate()");
        }
    }
}
