using System.Linq;
using ComicShop.Domain.Features.Comics;
using ComicShop.Domain.Features.Publishers;
using ComicShop.Domain.Features.Users;
using Microsoft.EntityFrameworkCore;

namespace ComicShop.Infra.Data.Contexts
{
    public class ComicShopCommonDbContext : DbContext
    {
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<ComicBook> ComicBooks { get; set; }
        public DbSet<User> Users { get; set; }

        public ComicShopCommonDbContext(DbContextOptions<ComicShopCommonDbContext> options) : base(options)
        {
            TryApplyMigration(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        private void TryApplyMigration(DbContextOptions<ComicShopCommonDbContext> options)
        {
            var inMemoryConfiguration = options.Extensions.FirstOrDefault(x => x.ToString().Contains("InMemoryOptionsExtension"));

            if (inMemoryConfiguration == null && Database.GetPendingMigrations().Count() > 0)
                Database.Migrate();
        }
    }
}
