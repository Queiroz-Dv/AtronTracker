using Atron.Tracker.Domain.Componentes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Tracker.Infrastructure.EntitiesConfiguration.ControleDeAcessoConfiguration
{
    public class PropriedadesDeFluxoModuloConfiguration : IEntityTypeConfiguration<PropriedadeDeFluxoModulo>
    {
        public void Configure(EntityTypeBuilder<PropriedadeDeFluxoModulo> builder)
        {
            builder.HasKey(key => new
            {
                key.ModuloId,
                key.ModuloCodigo,
                key.PropriedadeDeFluxoId,
                key.PropriedadeDeFluxoCodigo
            });

            builder.HasOne(pdfm => pdfm.Modulo)
                   .WithMany(m => m.PropriedadeDeFluxoModulos)
                   .HasForeignKey(pdfm => new { pdfm.ModuloId, pdfm.ModuloCodigo });
        }
    }
}