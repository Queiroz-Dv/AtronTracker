using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atron.Infrastructure.Context
{
    public class AtronDbContext : DbContext
    {
        public AtronDbContext(DbContextOptions<AtronDbContext> options) : base(options) { }

        public DbSet<Departamento> Departamentos { get; set; }

        public DbSet<Cargo> Cargos { get; set; }

        public DbSet<Mes> Meses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtronDbContext).Assembly);
        }
    }
}