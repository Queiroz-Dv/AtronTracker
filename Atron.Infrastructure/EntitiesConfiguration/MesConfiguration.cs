using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class MesConfiguration : IEntityTypeConfiguration<Mes>
    {
        public void Configure(EntityTypeBuilder<Mes> builder)
        {

            builder.HasKey(mth => mth.MesId);

            builder.Property(mth => mth.Descricao)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.HasData(new Mes { MesId = 1, Descricao = "Janeiro" },
                            new Mes { MesId = 2, Descricao = "Fevereiro" },
                            new Mes { MesId = 3, Descricao = "Março" },
                            new Mes { MesId = 4, Descricao = "Abril" },
                            new Mes { MesId = 5, Descricao = "Maio" },
                            new Mes { MesId = 6, Descricao = "Junho" },
                            new Mes { MesId = 7, Descricao = "Julho" },
                            new Mes { MesId = 8, Descricao = "Agosto" },
                            new Mes { MesId = 9, Descricao = "Setembro" },
                            new Mes { MesId = 10, Descricao = "Outubro" },
                            new Mes { MesId = 11, Descricao = "Novembro" },
                            new Mes { MesId = 12, Descricao = "Dezembro" });
        }
    }
}