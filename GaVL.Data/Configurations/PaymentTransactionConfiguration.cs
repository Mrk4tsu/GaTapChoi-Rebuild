using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("payment_transactions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
            builder.Property(x => x.SepayId).HasColumnName("sepay_id");
            builder.Property(x => x.Gateway).HasColumnName("gateway");
            builder.Property(x => x.TransactionDate).HasColumnName("transaction_date");
            builder.Property(x => x.AccountNumber).HasColumnName("account_number");
            builder.Property(x => x.SubAccount).HasColumnName("sub_account");
            builder.Property(x => x.AmountIn).HasColumnName("amount_in").HasColumnType("decimal(18,2)");
            builder.Property(x => x.AmountOut).HasColumnName("amount_out").HasColumnType("decimal(18,2)");
            builder.Property(x => x.Accumulated).HasColumnName("accumulated").HasColumnType("decimal(18,2)");
            builder.Property(x => x.Code).HasColumnName("code");
            builder.Property(x => x.TransactionContent).HasColumnName("transaction_content");
            builder.Property(x => x.ReferenceNumber).HasColumnName("reference_number");
            builder.Property(x => x.Body).HasColumnName("body");
        }
    }
}
