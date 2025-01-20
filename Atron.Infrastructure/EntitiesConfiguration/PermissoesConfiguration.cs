using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class PermissoesConfiguration : IEntityTypeConfiguration<Permissao>
    {
        public void Configure(EntityTypeBuilder<Permissao> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(pms => pms.UsuarioId).IsRequired();
            builder.Property(pms =>pms.UsuarioCodigo).IsRequired().HasMaxLength(10);
            builder.Property(pms => pms.DataInicial).IsRequired();
            builder.Property(pms => pms.DataFinal).IsRequired();
            builder.Property(pms => pms.Descricao).HasMaxLength(2500);
            builder.Property(pms => pms.PermissaoEstadoId).IsRequired();
            builder.Property(pms => pms.QuantidadeDeDias).IsRequired().HasMaxLength(365);
        }
    }
}