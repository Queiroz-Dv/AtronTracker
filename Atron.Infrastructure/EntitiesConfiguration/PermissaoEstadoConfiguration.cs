using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class PermissaoEstadoConfiguration : IEntityTypeConfiguration<PermissaoEstado>
    {
        public void Configure(EntityTypeBuilder<PermissaoEstado> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(pme => pme.Descricao).IsRequired().HasMaxLength(25);

            builder.HasData(new PermissaoEstado { Id = 1, Descricao = "Em atividade" },
                new PermissaoEstado { Id = 2, Descricao = "Aprovada" },
                new PermissaoEstado { Id = 3, Descricao = "Desaprovada" });
        }
    }
}