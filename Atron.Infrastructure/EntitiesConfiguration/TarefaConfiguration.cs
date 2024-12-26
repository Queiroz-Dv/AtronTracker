using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class TarefaConfiguration : IEntityTypeConfiguration<Tarefa>
    {
        public void Configure(EntityTypeBuilder<Tarefa> builder)
        {
            builder.HasKey(key => key.Id);
            builder.Property(dpt => dpt.IdSequencial).IsRequired();

            builder.Property(trf => trf.Titulo).IsRequired().HasMaxLength(50);
            builder.Property(trf => trf.Conteudo).HasMaxLength(2500);
            builder.Property(trf => trf.DataInicial); 
            builder.Property(trf => trf.DataFinal);

            builder.Property(trf => trf.TarefaEstadoId).IsRequired();

            builder.HasOne(usr => usr.Usuario)
                   .WithMany(trf => trf.Tarefas)
                   .HasForeignKey(usr => new { usr.UsuarioId, usr.UsuarioCodigo })
                   .HasPrincipalKey(usr => new { usr.Id, usr.Codigo });
        }
    }
}