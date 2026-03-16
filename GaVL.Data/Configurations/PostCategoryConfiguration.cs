using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class PostCategoryConfiguration : IEntityTypeConfiguration<PostCategory>
    {
        public void Configure(EntityTypeBuilder<PostCategory> builder)
        {
            builder.ToTable("post_categories");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(255);
            builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
            builder.Property(x => x.SeoAlias).HasColumnName("seo_alias").IsRequired().HasMaxLength(255);
            builder.Property(x => x.CreateAt).HasColumnName("create_at").IsRequired();
            builder.Property(x => x.UpdateAt).HasColumnName("update_at").IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();

            builder.HasMany(x => x.Posts)
                   .WithOne(x => x.Category)
                   .HasForeignKey(x => x.CategoryId);

            builder.HasIndex(x => x.SeoAlias).IsUnique(true);
            builder.HasIndex(x => x.Id);
            builder.HasIndex(x => x.IsDeleted);
        }
    }
}
