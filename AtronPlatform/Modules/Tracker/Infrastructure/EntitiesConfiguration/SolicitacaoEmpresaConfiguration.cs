using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration;

public sealed class SolicitacaoEmpresaConfiguration : IEntityTypeConfiguration<SolicitacaoEmpresa>
{
    public void Configure(EntityTypeBuilder<SolicitacaoEmpresa> builder)
    {
        builder.HasKey(solicitacao => solicitacao.Id);
        builder.Property(solicitacao => solicitacao.UsuarioCodigo).IsRequired().HasMaxLength(10);
        builder.Property(solicitacao => solicitacao.Status).IsRequired();
        builder.Property(solicitacao => solicitacao.CriadaEm).IsRequired();

        builder.HasIndex(solicitacao => new
        {
            solicitacao.EmpresaId,
            solicitacao.UsuarioId,
            solicitacao.UsuarioCodigo,
            solicitacao.Status
        }).IsUnique();

        builder.HasOne(solicitacao => solicitacao.Empresa)
            .WithMany()
            .HasForeignKey(solicitacao => solicitacao.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(solicitacao => solicitacao.Usuario)
            .WithMany()
            .HasForeignKey(solicitacao => new { solicitacao.UsuarioId, solicitacao.UsuarioCodigo })
            .HasPrincipalKey(usuario => new { usuario.Id, usuario.Codigo })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
