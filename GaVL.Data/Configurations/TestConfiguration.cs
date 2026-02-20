using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class TestConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            builder.ToTable("tests");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("id").IsRequired();
            builder.Property(t => t.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
        }
    }
}
