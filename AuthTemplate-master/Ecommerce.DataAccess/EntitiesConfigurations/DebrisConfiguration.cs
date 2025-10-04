using Ecommerce.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.Configurations
{
    public class DebrisConfiguration : IEntityTypeConfiguration<Debris>
    {
        public void Configure(EntityTypeBuilder<Debris> builder)
        {

            builder.HasKey(d => d.Id);

            builder.Property(d => d.NoradId)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasIndex(d => d.NoradId).IsUnique();

            builder.Property(d => d.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.TleLine1)
                   .IsRequired()
                   .HasMaxLength(80); // standard TLE line length

            builder.Property(d => d.TleLine2)
                   .IsRequired()
                   .HasMaxLength(80);

            builder.Property(d => d.LastFetchedAt)
                   .IsRequired();
        }
    }
}
