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

            builder.HasMany(pfa => pfa.PerfilDeAcessoModulos)
                   .WithOne(pam => pam.PerfilDeAcesso)
                   .HasForeignKey(pam => new { pam.PerfilDeAcessoId, pam.PerfilDeAcessoCodigo });
        }
    }
}