using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ecommerce.Entities.Models.Auth.Users;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Utilities.Enums;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);

        // Configure the Id as a string with max length matching IdentityUser
        builder.Property(c => c.Id)
            .HasMaxLength(450); // IdentityUser Ids are typically 450 chars

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Organization)
            .HasMaxLength(100);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        // Configure the one-to-one relationship with User
        builder.HasOne(c => c.User)
            .WithOne(u => u.Client)
            .HasForeignKey<Client>(c => c.Id)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationships
        builder.HasMany(c => c.Projects)
            .WithOne() // Assuming Project has a Client property
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Orders)
            .WithOne() // Assuming Order has a Client property
            .HasForeignKey(o => o.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Reviews)
            .WithOne() // Assuming Review has a Client property
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
