using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("id").UseIdentityColumn();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100).HasColumnName("name");
            builder.Property(c => c.Description).HasMaxLength(500).HasColumnName("description");

            //Index
            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
