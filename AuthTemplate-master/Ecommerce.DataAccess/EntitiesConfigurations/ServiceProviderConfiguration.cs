using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ecommerce.Entities.Models.Auth.Users;
using System.Text.Json;

public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
{
    public void Configure(EntityTypeBuilder<ServiceProvider> builder)
    {
        builder.HasKey(sp => sp.Id);
        builder.Property(sp => sp.Id).HasMaxLength(450);
        builder.Property(sp => sp.CompanyName).IsRequired().HasMaxLength(100);
        builder.Property(sp => sp.WebsiteUrl).HasMaxLength(200);

        builder.Property(sp => sp.CertificationsUrls)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
            );

        builder.Property(sp => sp.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(sp => sp.CreatedAt).IsRequired();
        builder.Property(sp => sp.UpdatedAt);

        builder.HasOne(sp => sp.User)
            .WithOne(u => u.ServiceProvider)
            .HasForeignKey<ServiceProvider>(sp => sp.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sp => sp.Services)
            .WithOne(s => s.Provider)
            .HasForeignKey(s => s.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sp => sp.Datasets)
            .WithOne(d => d.Provider)
            .HasForeignKey(d => d.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sp => sp.Withdrawals)
            .WithOne(w => w.Provider)
            .HasForeignKey(w => w.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sp => sp.Transactions)
            .WithOne(t => t.ServiceProvider)
            .HasForeignKey(t => t.ServiceProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
