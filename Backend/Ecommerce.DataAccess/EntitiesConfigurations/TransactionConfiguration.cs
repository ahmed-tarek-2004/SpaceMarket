using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ecommerce.Entities.Models;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Amount).HasColumnType("decimal(18,2)");
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(t => t.Date).IsRequired();

        builder.HasOne(t => t.Order)
            .WithOne(o => o.Transaction)
            .HasForeignKey<Order>(o => o.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Client)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.ClientId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(t => t.ServiceProvider)
            .WithMany(sp => sp.Transactions)
            .HasForeignKey(t => t.ServiceProviderId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
