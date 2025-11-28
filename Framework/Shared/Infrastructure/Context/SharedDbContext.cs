using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;

namespace Shared.Infrastructure.Context
{
    public class SharedDbContext(DbContextOptions<SharedDbContext> options) : DbContext(options)
    {
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Historico> Historicos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.HasSequence<long>("HistoricoSeq")
                .StartsAt(1)
                .IncrementsBy(1);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharedDbContext).Assembly);
        }
    }
}