using Atron.Domain.Componentes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration.ControleDeAcessoConfiguration
{
    public class PropriedadeFluxoConfiguration : IEntityTypeConfiguration<PropriedadesDeFluxo>
    {
        public void Configure(EntityTypeBuilder<PropriedadesDeFluxo> builder)
        {
            builder.HasKey(key => new { key.Id, key.Codigo });
            builder.Property(pdf => pdf.Id).ValueGeneratedOnAdd();

            builder.Property(pdf => pdf.Codigo).HasMaxLength(25);

            // Mapeamento de 1:N
            builder.HasMany(pdf => pdf.PropriedadesDeFluxoModulo)
                   .WithOne(pdfm => pdfm.PropriedadesDeFluxo)
                   .HasForeignKey(pdfm => new { pdfm.PropriedadeDeFluxoId, pdfm.PropriedadeDeFluxoCodigo });
        }
    }
}