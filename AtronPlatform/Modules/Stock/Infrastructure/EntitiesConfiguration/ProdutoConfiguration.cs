using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtronStock.Infrastructure.EntitiesConfiguration
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Codigo)
                .HasMaxLength(25)
                .IsRequired();

            builder.Property(p => p.Descricao)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.PrecoUnitario)
                .HasPrecision(18, 2);

            builder.Property(p => p.Status)
                .HasDefaultValue(EStatusProduto.Ativo)
                .IsRequired();

            builder.HasIndex(p => p.Codigo)
                .IsUnique();

            builder.HasOne(p => p.LoteProduto)
                .WithMany(lote => lote.Produtos)
                .HasForeignKey(p => p.LoteProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
