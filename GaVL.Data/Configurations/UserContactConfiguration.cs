using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class UserContactConfiguration : IEntityTypeConfiguration<UserContact>
    {
        public void Configure(EntityTypeBuilder<UserContact> builder)
        {
            builder.ToTable("user_contacts");
            
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd()
                   .HasColumnName("id");

            builder.Property(x => x.UserId)
                    .HasColumnName("user_id");

            builder.Property(x => x.Provider)
                   .IsRequired()
                   .HasColumnName("provider")
                   .HasMaxLength(50);
                   
            builder.Property(x => x.Value)
                   .IsRequired()
                   .HasColumnName("value")
                   .HasMaxLength(500);
                   
            builder.Property(x => x.DisplayLabel)
                   .HasColumnName("display_label")
                   .HasMaxLength(100);

            builder.Property(x => x.IsPublic)
                   .HasColumnName("is_public")
                   .HasDefaultValue(true);

            builder.Property(x => x.Position)
                   .HasColumnName("position")
                   .HasDefaultValue(0);

            builder.Property(x => x.CreatedAt)
                   .HasColumnName("created_at")
                   .HasDefaultValue(DateTime.UtcNow);

            builder.Property(x => x.UpdatedAt)
                   .HasColumnName("updated_at");

            builder.HasOne(x => x.User)
                   .WithMany(u => u.Contacts)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
