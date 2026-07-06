using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public class TarefaConfiguration : IEntityTypeConfiguration<Tarefa>
    {
        public void Configure(EntityTypeBuilder<Tarefa> builder)
        {
            builder.HasKey(key => key.Id);

            builder.Property(trf => trf.Identificador);
            builder.HasIndex(trf => trf.Identificador);

            builder.Property(trf => trf.DestinoInicial)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.Property(trf => trf.ExigeAprovacaoParaObter)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(trf => trf.UsuarioCodigo).HasMaxLength(10).IsRequired(false);
            builder.Property(trf => trf.DepartamentoCodigo).HasMaxLength(10).IsRequired(false);
            builder.Property(trf => trf.CargoCodigo).HasMaxLength(10).IsRequired(false);

            builder.Property(trf => trf.Titulo).IsRequired().HasMaxLength(50);
            builder.Property(trf => trf.Conteudo).HasMaxLength(2500);
            builder.Property(trf => trf.DataInicial); 
            builder.Property(trf => trf.DataFinal);

            builder.Property(trf => trf.TarefaEstadoId).IsRequired();

            builder.HasOne(trf => trf.EstadoDaTarefa)
                   .WithMany()
                   .HasForeignKey(trf => trf.TarefaEstadoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(usr => usr.Usuario)
                   .WithMany(trf => trf.Tarefas)
                   .HasForeignKey(usr => new { usr.UsuarioId, usr.UsuarioCodigo })
                   .HasPrincipalKey(usr => new { usr.Id, usr.Codigo })
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasOne(trf => trf.Departamento)
                   .WithMany()
                   .HasForeignKey(trf => new { trf.DepartamentoId, trf.DepartamentoCodigo })
                   .HasPrincipalKey(dpt => new { dpt.Id, dpt.Codigo })
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasOne(trf => trf.Cargo)
                   .WithMany()
                   .HasForeignKey(trf => new { trf.CargoId, trf.CargoCodigo })
                   .HasPrincipalKey(crg => new { crg.Id, crg.Codigo })
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);
        }
    }
}
