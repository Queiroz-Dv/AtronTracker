using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public class ProdutoCategoriaConfiguration : IEntityTypeConfiguration<ProdutoCategoria>
    {
        public void Configure(EntityTypeBuilder<ProdutoCategoria> builder)
        {
            builder.ToTable("ProdutoCategoria");

            builder.HasKey(pc => new { pc.ProdutoId, pc.CategoriaId });

            builder.HasOne(pc => pc.Produto)
                .WithMany(p => p.Categorias)
                .HasForeignKey(pc => pc.ProdutoId);

            builder.HasOne(pc => pc.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(pc => pc.CategoriaId);
        }
    }
}