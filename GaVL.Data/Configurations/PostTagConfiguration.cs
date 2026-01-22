using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class PostTagConfiguration : IEntityTypeConfiguration<PostTag>
    {
        public void Configure(EntityTypeBuilder<PostTag> builder)
        {
            builder.ToTable("post_tags");
            builder.HasKey(pt => new { pt.PostId, pt.TagId });
            builder.Property(pt => pt.PostId).HasColumnName("post_id").IsRequired();
            builder.Property(pt => pt.TagId).HasColumnName("tag_id").IsRequired();
            builder.HasOne(pt => pt.Post)
                   .WithMany(p => p.PostTags)
                   .HasForeignKey(pt => pt.PostId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pt => pt.Tag)
                   .WithMany(t => t.PostTags)
                   .HasForeignKey(pt => pt.TagId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(pt => pt.PostId);
            builder.HasIndex(pt => pt.TagId);
        }
    }
}
