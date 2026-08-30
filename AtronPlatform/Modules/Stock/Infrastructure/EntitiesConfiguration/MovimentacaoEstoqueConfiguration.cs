using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Extensions;

namespace AtronStock.Infrastructure.EntitiesConfiguration
{
    public class MovimentacaoEstoqueConfiguration : IEntityTypeConfiguration<MovimentacaoEstoque>
    {
        public void Configure(EntityTypeBuilder<MovimentacaoEstoque> builder)
        {
            builder.ToTable("MovimentacoesEstoque");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Quantidade)
                .IsRequired();

            builder.Property(m => m.TipoMovimentacao)
                .HasConversion(EnumStringConverter.Create<AtronStock.Domain.Enums.TipoMovimentacao>())
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(m => m.DataMovimentacao)
                .IsRequired();

            builder.Property(m => m.Observacao)
                .HasMaxLength(200);

            builder.Property(m => m.Origem)
                .HasMaxLength(100);

            builder.Property(m => m.TransacaoId)
                .IsRequired(false);

            builder.HasOne(m => m.Estoque)
                .WithMany()
                .HasForeignKey(m => m.EstoqueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
