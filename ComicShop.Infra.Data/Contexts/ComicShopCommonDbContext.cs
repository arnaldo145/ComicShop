﻿using ComicShop.Domain.Comics;
using ComicShop.Domain.Publishers;
using ComicShop.Infra.Data.Features.Comics;
using ComicShop.Infra.Data.Features.Publishers;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ComicShop.Infra.Data.Contexts
{
    public class ComicShopCommonDbContext : DbContext
    {
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<ComicBook> ComicBooks { get; set; }

        public ComicShopCommonDbContext(DbContextOptions<ComicShopCommonDbContext> options) : base(options)
        {
            TryApplyMigration(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PublisherEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ComicBookEntityConfiguration());

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
