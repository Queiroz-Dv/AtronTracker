using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtronStock.Infrastructure.Context
{
    public class StockDbContext(DbContextOptions<StockDbContext> options) : DbContext(options)
    {
        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StockDbContext).Assembly);
        }
    }
}