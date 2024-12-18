using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class TarefaConfiguration : IEntityTypeConfiguration<Tarefa>
    {
        public void Configure(EntityTypeBuilder<Tarefa> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(dpt => dpt.IdSequencial).IsRequired();

            builder.Property(usr => usr.Id).IsRequired();
            builder.Property(usr => usr.UsuarioCodigo).IsRequired().HasMaxLength(10);

            builder.Property(trf => trf.Titulo).IsRequired().HasMaxLength(50);
            builder.Property(trf => trf.Conteudo).HasMaxLength(2500);
            builder.Property(trf => trf.DataInicial);
            builder.Property(trf => trf.DataFinal);
            

            builder.HasOne(tre => tre.TarefaEstado)
                   .WithMany(trf => trf.Tarefas)
                   .HasForeignKey(fk => fk.EstadoDaTarefaId);
        }
    }
}