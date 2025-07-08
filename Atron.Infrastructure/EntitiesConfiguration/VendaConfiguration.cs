using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class VendaConfiguration : IEntityTypeConfiguration<Venda>
    {
        public void Configure(EntityTypeBuilder<Venda> builder)
        {
            builder.ToTable("Vendas");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.DataVenda).IsRequired();

            builder.Property(v => v.QuantidadeDeProdutoVendido)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(v => v.PrecoDoProduto)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(v => v.Removido)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(v => v.RemovidoEm)
                .HasColumnType("datetime");

            builder.HasOne(v => v.Produto)
                .WithMany(p => p.Vendas)
                .HasForeignKey(v => new { v.ProdutoId, v.ProdutoCodigo })
                .HasPrincipalKey(p => new { p.Id, p.Codigo })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Categoria)
                .WithMany(c => c.Vendas)
                .HasForeignKey(v => new { v.CategoriaId, v.CategoriaCodigo })
                .HasPrincipalKey(c => new { c.Id, c.Codigo })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Cliente)
                .WithMany(c => c.Vendas)
                .HasForeignKey(v => new { v.ClienteId, v.ClienteCodigo })
                .HasPrincipalKey(c => new { c.Id, c.Codigo })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}