using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");
            builder.HasKey(r => r.Id);
            builder.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
            builder.Property(r => r.Name).IsRequired().HasMaxLength(50).HasColumnName("name");
            builder.Property(r => r.Description).HasMaxLength(200).HasColumnName("description");

            //Index
            builder.HasIndex(r => r.Name).IsUnique();
        }
    }
}
