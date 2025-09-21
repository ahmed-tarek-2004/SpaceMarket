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
    public class DebrisConfiguration : IEntityTypeConfiguration<Debris>
    {
        public void Configure(EntityTypeBuilder<Debris> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.NoradId).IsRequired().HasMaxLength(50);
            builder.Property(d => d.Name).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Altitude).IsRequired();
            builder.Property(d => d.Velocity).IsRequired();
            builder.Property(d => d.LastFetchedAt).IsRequired();
        }
    }
}
