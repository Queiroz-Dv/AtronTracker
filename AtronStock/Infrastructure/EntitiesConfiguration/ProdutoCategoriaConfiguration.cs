using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtronStock.Infrastructure.EntitiesConfiguration
{
    public class ProdutoCategoriaConfiguration : IEntityTypeConfiguration<ProdutoCategoria>
    {
        public void Configure(EntityTypeBuilder<ProdutoCategoria> builder)
        {
            builder.HasKey(pc => new { pc.ProdutoId, pc.CategoriaId });

            builder.HasOne(pc => pc.Produto)
                .WithMany(p => p.Categorias)
                .HasForeignKey(pc => pc.ProdutoId);

            builder.HasOne(pc => pc.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(pc => pc.CategoriaId);

            builder.Property(pc => pc.ProdutoCodigo).HasMaxLength(25).IsRequired();
            builder.Property(pc => pc.CategoriaCodigo).HasMaxLength(25).IsRequired();
        }
    }
}
