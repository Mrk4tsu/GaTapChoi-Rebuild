using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class ModConfiguration : IEntityTypeConfiguration<Mod>
    {
        public void Configure(EntityTypeBuilder<Mod> builder)
        {
            builder.ToTable("mods");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnName("id").UseIdentityColumn();
            builder.Property(m => m.Name).IsRequired().HasMaxLength(150).HasColumnName("name");
            builder.Property(m => m.Description).HasMaxLength(1000).HasColumnName("description");
            builder.Property(m => m.IsPrivate).IsRequired().HasMaxLength(50).HasColumnName("is_private");
            builder.Property(m => m.IsDeleted).IsRequired().HasColumnName("is_deleted");
            builder.Property(m => m.IsLocked).IsRequired().HasColumnName("is_locked");
            builder.Property(m => m.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(m => m.UpdatedAt).IsRequired().HasColumnName("updated_at");
            builder.Property(m => m.UserId).IsRequired().HasColumnName("user_id");
            builder.Property(m => m.ViewCount).IsRequired().HasColumnName("view_count");
            builder.Property(m => m.CategoryId).IsRequired().HasColumnName("category_id");
            builder.Property(m => m.CrackType).IsRequired().HasColumnName("crack_type");
            builder.Property(m => m.SeoAlias).IsRequired().HasMaxLength(200).HasColumnName("seo_alias");
            // Relationships
            builder.HasOne(m => m.User).WithMany(u => u.Mods).HasForeignKey(m => m.UserId);
            builder.HasOne(m => m.Category).WithMany(c => c.Mods).HasForeignKey(m => m.CategoryId);

            // Indexes
            builder.HasIndex(m => m.Name);
            builder.HasIndex(x => x.IsDeleted);
            builder.HasIndex(x => x.IsPrivate);
            builder.HasIndex(x => x.UserId);
        }
    }
}
