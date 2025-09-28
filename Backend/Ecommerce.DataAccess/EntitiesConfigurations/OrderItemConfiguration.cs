using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ecommerce.Entities.Models;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Quantity).IsRequired();
        builder.Property(oi => oi.PriceSnapshot).HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.DownloadLink).HasMaxLength(1000);
        builder.Property(oi => oi.ApiKey).HasMaxLength(200);

        builder.HasOne(oi => oi.Order)
            .WithOne(o => o.Item)
            .HasForeignKey<Order>(o => o.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oi => oi.Service)
            .WithMany(s => s.OrderItems)
            .HasForeignKey(oi => oi.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oi => oi.Dataset)
            .WithMany()
            .HasForeignKey(oi => oi.DatasetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
