using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("tags");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(255);
            builder.Property(x => x.SeoAlias).HasColumnName("seo_alias").IsRequired().HasMaxLength(255);

            builder.HasIndex(x => x.SeoAlias).IsUnique();
        }
    }
}
