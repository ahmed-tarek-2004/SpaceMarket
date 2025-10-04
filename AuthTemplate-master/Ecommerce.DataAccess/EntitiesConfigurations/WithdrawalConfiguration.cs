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
    public class WithdrawalConfiguration : IEntityTypeConfiguration<Withdrawal>
    {
        public void Configure(EntityTypeBuilder<Withdrawal> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.ProviderId).IsRequired();
            builder.Property(w => w.Amount).HasColumnType("decimal(18,2)");
            builder.Property(w => w.BankAccount).IsRequired().HasMaxLength(100);

            // Enum as string with max length
            builder.Property(w => w.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(w => w.CreatedAt).IsRequired();
            builder.Property(w => w.UpdatedAt);

            // Many-to-One with ServiceProvider
            builder.HasOne(w => w.Provider)
                .WithMany(sp => sp.Withdrawals)
                .HasForeignKey(w => w.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
