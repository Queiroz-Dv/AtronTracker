using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Extensions;

namespace Infrastructure.EntitiesConfiguration
{
    public class SolicitacaoObtencaoTarefaConfiguration : IEntityTypeConfiguration<SolicitacaoObtencaoTarefa>
    {
        public void Configure(EntityTypeBuilder<SolicitacaoObtencaoTarefa> builder)
        {
            builder.HasKey(key => key.Id);

            builder.Property(sol => sol.SolicitanteCodigo).IsRequired().HasMaxLength(10);
            builder.Property(sol => sol.AprovadorCodigo).IsRequired().HasMaxLength(10);
            builder.Property(sol => sol.Status)
                .HasConversion(EnumStringConverter.Create<Domain.Enums.StatusSolicitacaoObtencaoTarefa>())
                .HasMaxLength(30)
                .IsRequired();
            builder.Property(sol => sol.DataSolicitacao).IsRequired();
            builder.Property(sol => sol.DataDecisao).IsRequired(false);

            builder.HasIndex(sol => new { sol.TarefaId, sol.Status });
            builder.HasIndex(sol => new { sol.AprovadorId, sol.AprovadorCodigo, sol.Status });

            builder.HasOne(sol => sol.Tarefa)
                   .WithMany(trf => trf.SolicitacoesObtencao)
                   .HasForeignKey(sol => sol.TarefaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sol => sol.Solicitante)
                   .WithMany()
                   .HasForeignKey(sol => new { sol.SolicitanteId, sol.SolicitanteCodigo })
                   .HasPrincipalKey(usr => new { usr.Id, usr.Codigo })
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sol => sol.Aprovador)
                   .WithMany()
                   .HasForeignKey(sol => new { sol.AprovadorId, sol.AprovadorCodigo })
                   .HasPrincipalKey(usr => new { usr.Id, usr.Codigo })
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
