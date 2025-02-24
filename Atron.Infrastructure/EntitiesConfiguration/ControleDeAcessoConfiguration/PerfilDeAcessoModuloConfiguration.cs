using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration.ControleDeAcessoConfiguration
{
    public class PerfilDeAcessoModuloConfiguration : IEntityTypeConfiguration<PerfilDeAcessoModulo>
    {
        public void Configure(EntityTypeBuilder<PerfilDeAcessoModulo> builder)
        {
            builder.HasKey(pam => new { pam.PerfilDeAcessoId, pam.PerfilDeAcessoCodigo, pam.ModuloId, pam.ModuloCodigo });

            builder.HasOne(pam => pam.PerfilDeAcesso)
                   .WithMany(pfa => pfa.PerfilDeAcessoModulos)
                   .HasForeignKey(pam => new { pam.PerfilDeAcessoId, pam.PerfilDeAcessoCodigo });

            builder.HasOne(pam => pam.Modulo)
                   .WithMany(mod => mod.PerfilDeAcessoModulos)
                   .HasForeignKey(pam => new { pam.ModuloId, pam.ModuloCodigo });
        }
    }

}
