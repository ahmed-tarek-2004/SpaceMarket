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
    public class ComplianceMessageConfiguration : IEntityTypeConfiguration<ComplianceMessage>
    {
        public void Configure(EntityTypeBuilder<ComplianceMessage> builder)
        {
            builder.HasKey(cm => cm.Id);

            builder.Property(cm => cm.SenderId).IsRequired().HasMaxLength(450);
            builder.Property(cm => cm.Text).IsRequired().HasMaxLength(1000);
            builder.Property(cm => cm.AttachmentUrl).HasMaxLength(500);
            builder.Property(cm => cm.CreatedAt).IsRequired();

            // Many-to-One with ComplianceTicket
            builder.HasOne(cm => cm.Ticket)
                .WithMany(ct => ct.Messages)
                .HasForeignKey(cm => cm.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-One with User
            builder.HasOne(cm => cm.Sender)
                .WithMany()
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
