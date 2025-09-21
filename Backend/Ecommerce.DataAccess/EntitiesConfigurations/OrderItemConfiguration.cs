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
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.PriceSnapshot).HasColumnType("decimal(18,2)");
            builder.Property(oi => oi.DownloadLink).HasMaxLength(500);
            builder.Property(oi => oi.ApiKey).HasMaxLength(100);

            // Many-to-One with Order
            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-One with Dataset
            builder.HasOne(oi => oi.Dataset)
                .WithMany()
                .HasForeignKey(oi => oi.DatasetId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete to preserve order history
        }
    }

}
