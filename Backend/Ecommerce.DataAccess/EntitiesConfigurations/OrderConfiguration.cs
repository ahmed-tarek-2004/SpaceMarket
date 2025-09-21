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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.ClientId).IsRequired().HasMaxLength(450);
            builder.Property(o => o.Amount).HasColumnType("decimal(18,2)");
            builder.Property(o => o.Commission).HasColumnType("decimal(18,2)");

            // Enum as string with max length
            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(o => o.CreatedAt).IsRequired();
            builder.Property(o => o.UpdatedAt);

            // Soft delete properties
            builder.Property(o => o.IsDeleted).HasDefaultValue(false);
            builder.HasQueryFilter(o => !o.IsDeleted);

            // Many-to-One with Client
            builder.HasOne(o => o.Client)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with OrderItems
            builder.HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
