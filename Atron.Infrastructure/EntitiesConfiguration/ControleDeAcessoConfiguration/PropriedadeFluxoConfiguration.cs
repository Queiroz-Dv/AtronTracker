using Atron.Tracker.Domain.Componentes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Tracker.Infrastructure.EntitiesConfiguration.ControleDeAcessoConfiguration
{
    public class PropriedadeFluxoConfiguration : IEntityTypeConfiguration<PropriedadesDeFluxo>
    {
        public void Configure(EntityTypeBuilder<PropriedadesDeFluxo> builder)
        {
            builder.HasKey(key => new { key.Id, key.Codigo });
            builder.Property(pdf => pdf.Id).ValueGeneratedOnAdd();

            builder.Property(pdf => pdf.Codigo).HasMaxLength(25);

            builder.HasData
                (
                    new PropriedadesDeFluxo { Id = 1, Codigo = "GRAVAR" },
                    new PropriedadesDeFluxo { Id = 2, Codigo = "CONSULTAR" },
                    new PropriedadesDeFluxo { Id = 3, Codigo = "DELETAR" },
                    new PropriedadesDeFluxo { Id = 4, Codigo = "ATUALIZAR" }
                );

            // Mapeamento de 1:N
            builder.HasMany(pdf => pdf.PropriedadesDeFluxoModulo)
                   .WithOne(pdfm => pdfm.PropriedadesDeFluxo)
                   .HasForeignKey(pdfm => new { pdfm.PropriedadeDeFluxoId, pdfm.PropriedadeDeFluxoCodigo });
        }
    }
}