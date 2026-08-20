using Application.DTO;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITarefaObtencaoService
    {
        Task<Resultado<IReadOnlyCollection<SolicitacaoObtencaoTarefaDTO>>> ObterSolicitacoesAsync();

        Task<Resultado<TarefaDTO>> AssumirAsync(int tarefaId);

        Task<Resultado<SolicitacaoObtencaoTarefaDTO>> SolicitarAsync(int tarefaId);

        Task<Resultado<SolicitacaoObtencaoTarefaDTO>> DecidirAsync(int solicitacaoId, bool aprovar);

        Task<Resultado<IReadOnlyCollection<TarefaDTO>>> ObterMeuQuadroAsync();

        Task<Resultado<IReadOnlyCollection<TarefaDTO>>> ObterEquipeAsync();

        Task<Resultado<IReadOnlyCollection<TarefaDTO>>> ObterDisponiveisAsync();

        Task<Resultado<TarefaAcessoDTO>> ObterAcessoAsync();
    }
}
