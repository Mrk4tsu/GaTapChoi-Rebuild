using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("posts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn().HasColumnName("id");
            builder.Property(x => x.SeoAlias).IsRequired().HasColumnName("seo_alias").HasMaxLength(250);
            builder.Property(x => x.Title).IsRequired().HasColumnName("title").HasMaxLength(250);
            builder.Property(x=>x.IsDeleted).HasDefaultValue(false).HasColumnName("is_deleted");
            builder.Property(x => x.CreateAt).HasColumnName("create_at").HasDefaultValue(DateTime.UtcNow);
            builder.Property(x => x.UpdateAt).HasColumnName("update_at").HasDefaultValue(DateTime.UtcNow);
            builder.Property(x => x.Description).HasColumnName("description").IsRequired();
            builder.Property(x => x.UserId).HasColumnName("user_id");
            builder.Property(x => x.MainImage).HasColumnName("main_image").HasMaxLength(250);
            builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(15);

            builder.HasOne(x => x.User).WithMany(x => x.Posts).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            //Index
            builder.HasIndex(x => x.IsDeleted);
            builder.HasIndex(x => x.Code);
        }
    }
}
