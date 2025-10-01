using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ecommerce.Entities.Models;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Quantity).IsRequired();
        builder.Property(ci => ci.PriceSnapshot).HasColumnType("decimal(18,2)");

        builder.HasOne(ci => ci.Client)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.ClientId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.HasOne(ci => ci.Service)
            .WithMany(s => s.CartItems)
            .HasForeignKey(ci => ci.ServiceId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade); 

        builder.HasOne(ci => ci.Dataset)
            .WithMany()
            .HasForeignKey(ci => ci.DatasetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relation with OrderItem (optional, because CartItem may not be ordered yet)
        builder.HasOne(ci => ci.Item)
            .WithMany()
            .HasForeignKey(ci => ci.OrderItemId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
