using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtronStock.Infrastructure.EntitiesConfiguration
{
    public sealed class LoteProdutoConfiguration : IEntityTypeConfiguration<LoteProduto>
    {
        public void Configure(EntityTypeBuilder<LoteProduto> builder)
        {
            builder.HasKey(lote => lote.Id);

            builder.Property(lote => lote.Codigo)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(lote => lote.Codigo)
                .IsUnique();
        }
    }
}
