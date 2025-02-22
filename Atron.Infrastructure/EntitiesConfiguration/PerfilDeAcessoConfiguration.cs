using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class PerfilDeAcessoConfiguration : IEntityTypeConfiguration<PerfilDeAcesso>
    {
        public void Configure(EntityTypeBuilder<PerfilDeAcesso> builder)
        {
            builder.HasKey(pfa => new { pfa.Id, pfa.Codigo });

            builder.Property(pfa => pfa.Id).ValueGeneratedOnAdd();

            builder.Property(pfa => pfa.Codigo).IsRequired().HasMaxLength(10);

            builder.Property(pfa => pfa.Descricao).IsRequired().HasMaxLength(50);

            builder.HasMany(pfa => pfa.Modulos)
                   .WithOne(mod => mod.PerfilDeAcesso)
                   .HasForeignKey(mod => new {mod.PerfilDeAcessoId, mod.PerfilDeAcessoCodigo }).HasConstraintName("FK_PERFILDEACESSO_ID");
        }
    }
}