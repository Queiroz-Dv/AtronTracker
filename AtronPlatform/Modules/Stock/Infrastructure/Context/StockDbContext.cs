using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtronStock.Infrastructure.Context
{
    public class StockDbContext(DbContextOptions<StockDbContext> options) : DbContext(options)
    {
        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Produto> Produtos { get; set; }

        public DbSet<LoteProduto> LotesProdutos { get; set; }

        public DbSet<ProcessamentoProdutoLote> ProcessamentosProdutosLote { get; set; }

        public DbSet<ProdutoCategoria> ProdutoCategorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StockDbContext).Assembly);

            if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                {
                    var propertyType = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;

                    if (propertyType == typeof(DateTime))
                        property.SetColumnType("timestamp without time zone");
                }
            }
        }
    }
}


