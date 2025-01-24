using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class MesConfiguration : IEntityTypeConfiguration<Mes>
    {
        public void Configure(EntityTypeBuilder<Mes> builder)
        {

            builder.HasKey(mth => mth.Id);

            builder.Property(mth => mth.Descricao)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.HasData(new Mes { Id = 1, Descricao = "Janeiro" },
                            new Mes { Id = 2, Descricao = "Fevereiro" },
                            new Mes { Id = 3, Descricao = "Março" },
                            new Mes { Id = 4, Descricao = "Abril" },
                            new Mes { Id = 5, Descricao = "Maio" },
                            new Mes { Id = 6, Descricao = "Junho" },
                            new Mes { Id = 7, Descricao = "Julho" },
                            new Mes { Id = 8, Descricao = "Agosto" },
                            new Mes { Id = 9, Descricao = "Setembro" },
                            new Mes { Id = 10, Descricao = "Outubro" },
                            new Mes { Id = 11, Descricao = "Novembro" },
                            new Mes { Id = 12, Descricao = "Dezembro" });
        }
    }
}