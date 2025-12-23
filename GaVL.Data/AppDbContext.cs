using GaVL.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GaVL.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Mod> Mods { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Url> Urls { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
