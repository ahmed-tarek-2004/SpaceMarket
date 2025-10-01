using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ecommerce.Entities.Models;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Title).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.Property(s => s.Price).HasColumnType("decimal(18,2)");
        builder.Property(s => s.ImagesUrl).HasMaxLength(1000);
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt);
        builder.Property(s => s.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasOne(s => s.Provider)
            .WithMany(sp => sp.Services)
            .HasForeignKey(s => s.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Category)
            .WithMany()
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.OrderItems)
            .WithOne(oi => oi.Service)
            .HasForeignKey(oi => oi.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.CartItems)
            .WithOne(ci => ci.Service)
            .HasForeignKey(ci => ci.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
