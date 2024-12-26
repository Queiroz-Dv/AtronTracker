using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class TarefaEstadoConfiguration : IEntityTypeConfiguration<TarefaEstado>
    {
        public void Configure(EntityTypeBuilder<TarefaEstado> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(tre => tre.Descricao).HasMaxLength(25);

            builder.HasData(new TarefaEstado { Id = 1, Descricao = "Em atividade" },
                            new TarefaEstado { Id = 2, Descricao = "Pendente de aprovação" },
                            new TarefaEstado { Id = 3, Descricao = "Entregue" },
                            new TarefaEstado { Id = 4, Descricao = "Finalizada"},
                            new TarefaEstado { Id = 5, Descricao = "Iniciada"});
                            
        }
    }
}
