using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.HasAlternateKey(p => p.Codigo);
            builder.HasIndex(p => p.Codigo)
                   .IsUnique()
                   .HasDatabaseName("IX_Produto_Codigo");

            builder.Property(p => p.Codigo)
                   .IsRequired()
                   .HasMaxLength(25);

            builder.Property(p => p.Descricao)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.Removido)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(p => p.RemovidoEm).IsRequired(false);

            builder.Property(p => p.QuantidadeEmEstoque)
                   .IsRequired()
                   .HasDefaultValue(0);
        }
    }
}
