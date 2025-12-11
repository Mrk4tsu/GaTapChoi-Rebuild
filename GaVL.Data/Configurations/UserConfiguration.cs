using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnName("id");
            builder.Property(u => u.Username).IsRequired().HasMaxLength(50).HasColumnName("username");
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100).HasColumnName("email");
            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(100).HasColumnName("password_hash");
            builder.Property(u => u.AvatarUrl).HasMaxLength(200).HasColumnName("avatar_url");
            builder.Property(u => u.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(u => u.IsActive).IsRequired().HasDefaultValue(false).HasColumnName("is_active");
            builder.Property(u => u.LastLoginAt).HasColumnName("last_login_at");
            builder.Property(u => u.RoleId).IsRequired().HasDefaultValue(4).HasColumnName("role_id");

            //Index
            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();

            //Relationships
            builder.HasOne(u => u.Role)
                   .WithMany(r => r.Users)
                   .HasForeignKey(u => u.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
