using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtronStock.Infrastructure.Context
{
    public class StockDbContext(DbContextOptions<StockDbContext> options) : DbContext(options)
    {
        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Estoque> Estoques { get; set; }

        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }

        public DbSet<Fornecedor> Fornecedores { get; set; }

        public DbSet<Entrada> Entradas { get; set; }

        public DbSet<ItemEntrada> ItensEntrada { get; set; }

        public DbSet<Venda> Vendas { get; set; }

        public DbSet<ItemVenda> ItensVenda { get; set; }

        public DbSet<Produto> Produtos { get; set; }

        public DbSet<ProdutoCategoria> ProdutoCategorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StockDbContext).Assembly);
        }
    }
}