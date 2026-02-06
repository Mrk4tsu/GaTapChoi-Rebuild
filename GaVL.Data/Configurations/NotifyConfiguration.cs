using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class NotifyConfiguration : IEntityTypeConfiguration<Notify>
    {
        public void Configure(EntityTypeBuilder<Notify> builder)
        {
            builder.ToTable("notifies");
            builder.HasKey(n => n.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasColumnName("id");
            builder.Property(n => n.Title)
                   .IsRequired()
                   .HasColumnName("title")
                   .HasMaxLength(200);
            builder.Property(n => n.Content)
                   .IsRequired()
                   .HasColumnName("content");
            builder.Property(n => n.CreatedAt)
                     .IsRequired()
                     .HasColumnName("created_at");
            builder.Property(n => n.LastUpdatedAt)
                        .IsRequired()
                        .HasColumnName("last_updated_at");
            builder.Property(n => n.UserId)
                        .IsRequired()
                        .HasColumnName("user_id");
            builder.Property(n => n.IsDeleted)
                        .HasColumnName("is_deleted")
                        .HasDefaultValue(false);
            builder.Property(x => x.Url).HasColumnName("url");
            builder.HasOne(n => n.User)
                     .WithMany(u => u.Notifies)
                     .HasForeignKey(n => n.UserId)
                     .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(n => n.UserId).HasDatabaseName("IX_Notifies_UserId");
            builder.HasIndex(n => n.IsDeleted).HasDatabaseName("IX_Notifies_IsDeleted");
        }
    }
}
