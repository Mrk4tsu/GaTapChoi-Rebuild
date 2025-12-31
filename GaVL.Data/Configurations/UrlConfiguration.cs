using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class UrlConfiguration : IEntityTypeConfiguration<Url>
    {
        public void Configure(EntityTypeBuilder<Url> builder)
        {
            builder.ToTable("urls");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(x => x.UrlString).HasColumnName("url_string").IsRequired().HasMaxLength(2048);
            builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
            builder.Property(x => x.ModId).HasColumnName("mod_id").IsRequired();
            builder.HasOne(x => x.Mod)
                   .WithMany(m => m.Urls)
                   .HasForeignKey(x => x.ModId)
                   .OnDelete(DeleteBehavior.Cascade);

            //Index
            builder.HasIndex(x => x.ModId);
        }
    }
}
