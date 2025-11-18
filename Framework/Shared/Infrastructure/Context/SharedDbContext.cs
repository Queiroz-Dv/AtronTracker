using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared.Infrastructure.Context
{
    public class SharedDbContext(DbContextOptions<SharedDbContext> options) : DbContext(options)
    {
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Historico> Historicos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("AtronShared");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharedDbContext).Assembly);
        }
    }
}