using Ecommerce.Entities.Models.Auth.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
{
    public void Configure(EntityTypeBuilder<ServiceProvider> builder)
    {
        builder.HasKey(sp => sp.Id);

        // Configure the Id as a string with max length matching IdentityUser
        builder.Property(sp => sp.Id)
            .HasMaxLength(450); // IdentityUser Ids are typically 450 chars

        builder.Property(sp => sp.CompanyName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sp => sp.WebsiteUrl)
            .HasMaxLength(200);

        builder.Property(sp => sp.CertificationsUrlJson)
            .HasMaxLength(1000);

        // Configure enum as string with max length
        builder.Property(sp => sp.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(sp => sp.CreatedAt)
            .IsRequired();

        builder.Property(sp => sp.UpdatedAt);

        // Configure the one-to-one relationship with User
        builder.HasOne(sp => sp.User)
            .WithOne(u => u.ServiceProvider)
            .HasForeignKey<ServiceProvider>(sp => sp.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationships
        builder.HasMany(sp => sp.Services)
            .WithOne() // Assuming Service has a Provider property
            .HasForeignKey(s => s.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sp => sp.Datasets)
            .WithOne() // Assuming Dataset has a Provider property
            .HasForeignKey(d => d.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sp => sp.Withdrawals)
            .WithOne() // Assuming Withdrawal has a Provider property
            .HasForeignKey(w => w.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}