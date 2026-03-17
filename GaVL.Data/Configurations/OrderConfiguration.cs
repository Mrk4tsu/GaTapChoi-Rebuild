using GaVL.Data.Entities;
using GaVL.Data.EntityTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasColumnName("id").IsRequired();
            builder.Property(o => o.Total)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("amount")
                .IsRequired();
            builder.Property(o => o.PaymentStatus).HasColumnName("payment_status").HasDefaultValue(PaymentType.UnPaid);
            builder.Property(o => o.Email).HasColumnName("email").IsRequired().HasMaxLength(250);
            builder.Property(o => o.NumberPhone).HasColumnName("number_phone").HasMaxLength(12);
            builder.Property(o => o.CreatedAt).HasColumnName("create_at").HasDefaultValue(DateTime.UtcNow);
        }
    }
}
