using Application.DTO;
using Application.Interfaces.Services;
using Application.UseCases.TarefaCases;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas.Obtencao
{
    public class TarefaObtencaoService(
        AssumirTarefaCase assumirTarefaCase,
        SolicitarTarefaCase solicitarTarefaCase,
        ObterSolicitacaoCase obterSolicitacaoCase,
        DecidirTarefaCase decidirTarefaCase,
        ObterMeuQuadroCase obterMeuQuadroCase,
        ObterEquipeCase obterEquipeCase,
        ObterTarefasDisponiveisCase obterTarefasDisponiveisCase,
        ObterAcessoTarefaCase obterAcessoTarefaCase) : ITarefaObtencaoService
    {
        private readonly AssumirTarefaCase _assumirTarefaCase = assumirTarefaCase;
        private readonly SolicitarTarefaCase _solicitarTarefaCase = solicitarTarefaCase;
        private readonly ObterSolicitacaoCase _obterSolicitacaoCase = obterSolicitacaoCase;
        private readonly DecidirTarefaCase _decidirTarefaCase = decidirTarefaCase;
        private readonly ObterMeuQuadroCase _obterMeuQuadroCase = obterMeuQuadroCase;
        private readonly ObterEquipeCase _obterEquipeCase = obterEquipeCase;
        private readonly ObterTarefasDisponiveisCase _obterTarefasDisponiveisCase = obterTarefasDisponiveisCase;
        private readonly ObterAcessoTarefaCase _obterAcessoTarefaCase = obterAcessoTarefaCase;

        public async Task<Resultado<IReadOnlyCollection<SolicitacaoObtencaoTarefaDTO>>> ObterSolicitacoesAsync()
        {
            return await _obterSolicitacaoCase.ExecutarAsync();
        }

        public async Task<Resultado<TarefaDTO>> AssumirAsync(int tarefaId)
        {
            return await _assumirTarefaCase.ExecutarAsync(tarefaId);
        }

        public async Task<Resultado<SolicitacaoObtencaoTarefaDTO>> SolicitarAsync(int tarefaId)
        {
            return await _solicitarTarefaCase.ExecutarAsync(tarefaId);
        }

        public async Task<Resultado<SolicitacaoObtencaoTarefaDTO>> DecidirAsync(int solicitacaoId, bool aprovar)
        {
            return await _decidirTarefaCase.ExecutarAsync(solicitacaoId, aprovar);
        }

        public Task<Resultado<IReadOnlyCollection<TarefaDTO>>> ObterMeuQuadroAsync()
            => _obterMeuQuadroCase.ExecutarAsync();

        public Task<Resultado<IReadOnlyCollection<TarefaDTO>>> ObterEquipeAsync()
            => _obterEquipeCase.ExecutarAsync();

        public Task<Resultado<IReadOnlyCollection<TarefaDTO>>> ObterDisponiveisAsync()
            => _obterTarefasDisponiveisCase.ExecutarAsync();

        public Task<Resultado<TarefaAcessoDTO>> ObterAcessoAsync()
            => _obterAcessoTarefaCase.ExecutarAsync();
    }
}