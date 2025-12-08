using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtronStock.Infrastructure.EntitiesConfiguration
{
    public class ItemEntradaConfiguration : IEntityTypeConfiguration<ItemEntrada>
    {
        public void Configure(EntityTypeBuilder<ItemEntrada> builder)
        {
            builder.ToTable("ItensEntrada");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.ProdutoCodigo)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(i => i.Quantidade)
                .IsRequired();

            builder.Property(i => i.PrecoCusto)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(i => i.Produto)
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
