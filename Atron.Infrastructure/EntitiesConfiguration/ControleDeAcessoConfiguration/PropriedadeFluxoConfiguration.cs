using Atron.Domain.Componentes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
<<<<<<< HEAD
=======
using System.Collections.Generic;
>>>>>>> 9c5d71f27b0cbf2d0952b4c244329a3ccae73954

namespace Atron.Infrastructure.EntitiesConfiguration.ControleDeAcessoConfiguration
{
    public class PropriedadeFluxoConfiguration : IEntityTypeConfiguration<PropriedadesDeFluxo>
    {
        public void Configure(EntityTypeBuilder<PropriedadesDeFluxo> builder)
        {
            builder.HasKey(key => new { key.Id, key.Codigo });
            builder.Property(pdf => pdf.Id).ValueGeneratedOnAdd();
<<<<<<< HEAD
            builder.Property(pdf => pdf.Codigo).HasMaxLength(25);           
=======
            builder.Property(pdf => pdf.Codigo).HasMaxLength(25);

            // Mapea as propriedades para os botões do sistema
            var dadosIniciais = new List<PropriedadesDeFluxo>
            {
                new() { Codigo = "SALVAR" },
                new() { Codigo = "EDITAR" },
                new() { Codigo = "EXCLUIR" },
                new() { Codigo = "VISUALIZAR" },
            };

            builder.HasData(dadosIniciais);

            // Mapeamento de 1:N
            builder.HasMany(pdf => pdf.PropriedadesDeFluxoModulo)
                   .WithOne(pdfm => pdfm.PropriedadesDeFluxo)
                   .HasForeignKey(pdfm => new { pdfm.PropriedadeDeFluxoId, pdfm.PropriedadeDeFluxoCodigo });
>>>>>>> 9c5d71f27b0cbf2d0952b4c244329a3ccae73954
        }
    }
}