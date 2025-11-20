using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Entities;

namespace Shared.Infrastructure.Configuration
{
    public class HistoricoConfiguration : IEntityTypeConfiguration<Historico>
    {
        public void Configure(EntityTypeBuilder<Historico> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id).ValueGeneratedOnAdd();

            // Configura o campo para usar o valor da Sequence por padrão
            builder.Property(h => h.CodigoHistorico)
                   .HasDefaultValueSql("NEXT VALUE FOR AtronShared.HistoricoSeq");
            
            builder.HasIndex(h => h.CodigoRegistro);
            builder.Property(h => h.CodigoRegistro).IsRequired().HasMaxLength(50);

            builder.Property(h => h.Descricao)
                   .IsRequired()
                   .HasMaxLength(1500);

            // Data de criação padrão
            builder.Property(h => h.DataCriacao)
                  .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}