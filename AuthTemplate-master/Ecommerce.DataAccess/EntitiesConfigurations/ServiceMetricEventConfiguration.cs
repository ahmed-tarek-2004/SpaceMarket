using Ecommerce.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.EntitiesConfigurations
{
    public class ServiceMetricEventConfiguration : IEntityTypeConfiguration<ServiceMetricEvent>
    {
        public void Configure(EntityTypeBuilder<ServiceMetricEvent> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.EventType)
                   .HasConversion<string>() 
                   .HasMaxLength(50);

            builder.Property(e => e.Timestamp).IsRequired();

            builder.HasOne(e => e.Service)
                   .WithMany() // ممكن نعمل WithMany(s => s.MetricEvents) لو ضفنا ICollection في Service
                   .HasForeignKey(e => e.ServiceId)
                   .OnDelete(DeleteBehavior.Cascade);

            //builder.Property(e => e.MetadataJson).HasMaxLength(2000);
        }
    }
}
