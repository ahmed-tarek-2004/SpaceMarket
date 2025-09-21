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
    public class ComplianceTicketConfiguration : IEntityTypeConfiguration<ComplianceTicket>
    {
        public void Configure(EntityTypeBuilder<ComplianceTicket> builder)
        {
            builder.HasKey(ct => ct.Id);

            builder.Property(ct => ct.Topic).IsRequired().HasMaxLength(100);
            builder.Property(ct => ct.Description).HasMaxLength(1000);
            builder.Property(ct => ct.AttachmentsUrl).HasMaxLength(500);
            builder.Property(ct => ct.PreferredDate).IsRequired();

            // Enum as string with max length
            builder.Property(ct => ct.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(ct => ct.CreatedAt).IsRequired();
            builder.Property(ct => ct.UpdatedAt);

            // Many-to-One with Client
            builder.HasOne(ct => ct.Client)
                .WithMany()
                .HasForeignKey(ct => ct.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-One with ServiceProvider
            builder.HasOne(ct => ct.Provider)
                .WithMany()
                .HasForeignKey(ct => ct.ProviderId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete to preserve ticket history

            // One-to-Many with ComplianceMessage
            builder.HasMany(ct => ct.Messages)
                .WithOne(cm => cm.Ticket)
                .HasForeignKey(cm => cm.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
