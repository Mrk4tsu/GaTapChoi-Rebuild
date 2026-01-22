using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GaVL.Data.Configurations
{
    public class PostRevisionConfiguration : IEntityTypeConfiguration<PostRevision>
    {
        public void Configure(EntityTypeBuilder<PostRevision> builder)
        {
            builder.ToTable("post_revisions");
            builder.HasKey(pr => pr.Id);
            builder.Property(pr => pr.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(pr => pr.PostId).HasColumnName("post_id").IsRequired();
            builder.Property(pr => pr.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(pr => pr.ContentSnapshot).HasColumnName("content_snapshot").IsRequired();
            builder.Property(pr => pr.CreatedAt).HasColumnName("created_at").IsRequired();

            builder.HasOne(pr => pr.User)
                   .WithMany(x => x.PostRevisions)
                   .HasForeignKey(pr => pr.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(pr => pr.Id);
            builder.HasIndex(pr => pr.PostId);
        }
    }
}
