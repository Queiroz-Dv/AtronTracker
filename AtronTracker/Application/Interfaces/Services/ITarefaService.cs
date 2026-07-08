using Application.DTO;
using Application.DTO.Request;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ITarefaService
    {
        Task<Resultado<List<TarefaDTO>>> ObterTodosAsync();

        Task<Resultado<List<TarefaDTO>>> ObterMeuQuadroAsync();

        Task<Resultado<List<TarefaDTO>>> ObterEquipeAsync();

        Task<Resultado<List<TarefaDTO>>> ObterDisponiveisAsync();

        Task<Resultado<List<SolicitacaoObtencaoTarefaDTO>>> ObterSolicitacoesAsync();

        Task<Resultado<List<TarefaEstadoDTO>>> ObterEstadosAsync();

        Task<Resultado<TarefaConfiguracoesDTO>> ObterConfiguracoesAsync();

        Task<Resultado<TarefaConfiguracoesDTO>> AtualizarConfiguracoesAsync(TarefaConfiguracoesRequest request);

        Task<Resultado<TarefaDTO>> CriarAsync(TarefaDTO tarefaDTO);

        Task<Resultado<TarefaDTO>> AtualizarAsync(int id, TarefaDTO tarefaDTO);

        Task<Resultado> ExcluirAsync(string id);

        Task<Resultado<TarefaDTO>> AssumirAsync(int id);

        Task<Resultado<SolicitacaoObtencaoTarefaDTO>> SolicitarObtencaoAsync(int id);

        Task<Resultado<SolicitacaoObtencaoTarefaDTO>> AprovarSolicitacaoAsync(int id);

        Task<Resultado<SolicitacaoObtencaoTarefaDTO>> RecusarSolicitacaoAsync(int id);

        Task<Resultado<TarefaDTO>> ObterPorId(int id);
    }
}
