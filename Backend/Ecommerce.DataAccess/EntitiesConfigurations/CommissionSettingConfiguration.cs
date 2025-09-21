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
    public class CommissionSettingConfiguration : IEntityTypeConfiguration<CommissionSetting>
    {
        public void Configure(EntityTypeBuilder<CommissionSetting> builder)
        {
            builder.HasKey(cs => cs.Id);

            builder.Property(cs => cs.RatePercent).HasColumnType("decimal(5,2)");

            // Many-to-One with ServiceCategory
            builder.HasOne(cs => cs.Category)
                .WithMany()
                .HasForeignKey(cs => cs.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
