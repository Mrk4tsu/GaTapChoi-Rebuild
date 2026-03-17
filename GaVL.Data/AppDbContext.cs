using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GaVL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasPostgresExtension("pg_trgm");
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Mod> Mods { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Url> Urls { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<PostRevision> PostRevisions { get; set; }
        public DbSet<Notify> Notifies { get; set; }
        public DbSet<PaymentTransaction> Payments { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
