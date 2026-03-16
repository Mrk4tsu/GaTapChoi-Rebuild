using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("comments");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd().HasColumnName("id");
            builder.Property(c => c.Content).IsRequired().HasColumnName("content");
            builder.Property(c => c.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(c => c.UpdateAt).IsRequired().HasColumnName("updated_at");
            builder.Property(c => c.UserId).IsRequired().HasColumnName("user_id");
            builder.Property(c => c.RootId).HasColumnName("root_id");
            builder.Property(c => c.ParentId).HasColumnName("parent_id");
            
            builder.HasOne(c => c.User)
                   .WithMany(u => u.Comments)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(c => c.CommentRoot)
                   .WithMany()
                   .HasForeignKey(c => c.RootId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.RootId);
            builder.HasIndex(c => c.ParentId);
            builder.HasIndex(c => c.CreatedAt);
        }
    }
}
