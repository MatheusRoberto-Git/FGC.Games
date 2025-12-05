using FGC.Games.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FGC.Games.Infrastructure.Data.Context
{
    public class GamesDbContext : DbContext
    {
        public GamesDbContext(DbContextOptions<GamesDbContext> options) : base(options) { }

        public GamesDbContext() : base() { }

        public DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GamesDbContext).Assembly);
        }
    }
}