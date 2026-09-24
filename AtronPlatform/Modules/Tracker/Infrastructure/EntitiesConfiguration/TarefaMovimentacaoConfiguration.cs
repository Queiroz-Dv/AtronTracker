using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Extensions;

namespace Infrastructure.EntitiesConfiguration
{
    public class TarefaMovimentacaoConfiguration : IEntityTypeConfiguration<TarefaMovimentacao>
    {
        public void Configure(EntityTypeBuilder<TarefaMovimentacao> builder)
        {
            builder.HasKey(movimentacao => movimentacao.Id);

            builder.Property(movimentacao => movimentacao.Tipo)
                .HasMaxLength(80)
                .HasConversion(
                    tipo => tipo.GetDescription(), 
                    descricao => ParseTipoMovimentacao(descricao)
                ).IsRequired();

            builder.Property(movimentacao => movimentacao.Descricao)
                .IsRequired()
                .HasMaxLength(1500);

            builder.Property(movimentacao => movimentacao.ResponsavelCodigo)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(movimentacao => movimentacao.ResponsavelNome)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(movimentacao => movimentacao.DataOcorrencia)
                .IsRequired();

            builder.HasIndex(movimentacao => new
            {
                movimentacao.TarefaId,
                movimentacao.DataOcorrencia
            });

            builder.HasOne(movimentacao => movimentacao.Tarefa)
                .WithMany(tarefa => tarefa.Movimentacoes)
                .HasForeignKey(movimentacao => movimentacao.TarefaId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static TipoMovimentacaoTarefa ParseTipoMovimentacao(string descricao)
        {
            foreach (TipoMovimentacaoTarefa tipo in Enum.GetValues(typeof(TipoMovimentacaoTarefa)))
            {
                if (tipo.GetDescription().Equals(descricao, StringComparison.OrdinalIgnoreCase) ||
                    tipo.ToString().Equals(descricao, StringComparison.OrdinalIgnoreCase))
                {
                    return tipo;
                }
            }
            return TipoMovimentacaoTarefa.Criacao;
        }
    }
}