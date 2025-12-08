using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtronStock.Infrastructure.EntitiesConfiguration
{
    public class EntradaConfiguration : IEntityTypeConfiguration<Entrada>
    {
        public void Configure(EntityTypeBuilder<Entrada> builder)
        {
            builder.ToTable("Entradas");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FornecedorCodigo)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(e => e.DataEntrada)
                .IsRequired();

            builder.Property(e => e.Observacao)
                .HasMaxLength(500);

            builder.HasOne(e => e.Fornecedor)
                .WithMany()
                .HasForeignKey(e => e.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Itens)
                .WithOne(i => i.Entrada)
                .HasForeignKey(i => i.EntradaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
