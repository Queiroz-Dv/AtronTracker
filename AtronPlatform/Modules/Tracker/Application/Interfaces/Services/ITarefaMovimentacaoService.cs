using Application.DTO;
using Domain.Entities;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITarefaMovimentacaoService
    {
        Task<Resultado> RegistrarCriacaoAsync(Tarefa tarefa, Usuario responsavel);

        Task<Resultado> RegistrarAtualizacaoAsync(Tarefa tarefaAnterior, Tarefa tarefaAtual, Usuario responsavel);

        Task<Resultado> RegistrarObtencaoAsync(Tarefa tarefa, Usuario responsavel);

        Task<Resultado> RegistrarSolicitacaoAsync(SolicitacaoObtencaoTarefa solicitacao, Usuario responsavel);

        Task<Resultado> RegistrarDecisaoAsync(
            SolicitacaoObtencaoTarefa solicitacao,
            Usuario responsavel,
            bool aprovar);

        Task<Resultado<TarefaMovimentacaoPaginaDTO>> ObterAsync(
            int tarefaId,
            int pagina,
            int tamanhoPagina);
    }
}
