using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ecommerce.Entities.Models;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Amount).HasColumnType("decimal(18,2)");
        builder.Property(o => o.Commission).HasColumnType("decimal(18,2)");

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt);

        builder.Property(o => o.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(o => !o.IsDeleted);

        builder.HasOne(o => o.Client)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Item)
            .WithOne()
            .HasForeignKey<Order>(o => o.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Transaction)
           .WithOne(t => t.Order)
           .HasForeignKey<Transaction>(t => t.OrderId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
