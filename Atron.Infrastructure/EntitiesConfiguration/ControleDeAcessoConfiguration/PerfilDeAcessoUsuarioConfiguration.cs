using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration.ControleDeAcessoConfiguration
{
    public class PerfilDeAcessoUsuarioConfiguration : IEntityTypeConfiguration<PerfilDeAcessoUsuario>
    {
        public void Configure(EntityTypeBuilder<PerfilDeAcessoUsuario> builder)
        {
            builder.HasKey(perf => new
            {
                perf.PerfilDeAcessoId,
                perf.PerfilDeAcessoCodigo,
                perf.UsuarioId,
                perf.UsuarioCodigo
            });

            builder.HasOne(perf => perf.Usuario)
                   .WithMany(usr => usr.PerfilDeAcessoUsuarios)
                   .HasForeignKey(perf => new { perf.UsuarioId, perf.UsuarioCodigo })
                   .HasPrincipalKey(usr => new { usr.Id, usr.Codigo });

            builder.HasOne(perf => perf.PerfilDeAcesso)
                     .WithMany(pfa => pfa.PerfilDeAcessoUsuarios)
                     .HasForeignKey(perf => new { perf.PerfilDeAcessoId, perf.PerfilDeAcessoCodigo })
                     .HasPrincipalKey(pfa => new { pfa.Id, pfa.Codigo });
        }
    }
}