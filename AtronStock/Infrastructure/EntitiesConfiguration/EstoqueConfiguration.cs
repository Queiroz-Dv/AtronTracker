using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtronStock.Infrastructure.EntitiesConfiguration
{
    public class EstoqueConfiguration : IEntityTypeConfiguration<Estoque>
    {
        public void Configure(EntityTypeBuilder<Estoque> builder)
        {
            builder.ToTable("Estoques");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Quantidade)
                .IsRequired();

            builder.Property(e => e.DataUltimaAtualizacao)
                .IsRequired();

            builder.HasOne(e => e.Produto)
                .WithOne() // Assuming 1:1 for now as per plan
                .HasForeignKey<Estoque>(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
