using Application.DTO;
using Application.UseCases.TarefaCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;
using Shared.Infrastructure.Filters;

namespace AtronPlatform.WebApi.Controllers.Tracker
{
    /// <summary>
    /// Controller para endpoints relacionados ao fluxo de obtenção, atribuição e aprovação de tarefas.
    /// </summary>
    [Route("api/Tarefa")]
    [ApiController]
    [Authorize(Policy = ModuloPolicies.Tarefa)]
    public class TarefaObtencaoController(
        ObterSolicitacaoCase obterSolicitacao,
        AssumirTarefaCase assumirTarefa,
        SolicitarTarefaCase solicitarTarefa,
        DecidirTarefaCase decidirTarefa) : ControllerBase
    {
        /// <summary>
        /// Obtém todas as solicitações de obtenção de tarefas pendentes de aprovação.
        /// </summary>
        [HttpGet("Solicitacoes")]
        public async Task<ActionResult<IEnumerable<SolicitacaoObtencaoTarefaDTO>>> ObterSolicitacoes()
        {
            var resultado = await obterSolicitacao.ExecutarAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Atribui a tarefa diretamente ao usuário autenticado (assumir tarefa disponível).
        /// </summary>
        [HttpPost("{id}/Assumir")]
        [Transactional]
        public async Task<ActionResult<TarefaDTO>> Assumir(int id)
        {
            var resultado = await assumirTarefa.ExecutarAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Cria uma solicitação para que o usuário autenticado possa obter a tarefa (requer aprovação).
        /// </summary>
        [HttpPost("{id}/SolicitarObtencao")]
        [Transactional]
        public async Task<ActionResult<SolicitacaoObtencaoTarefaDTO>> SolicitarObtencao(int id)
        {
            var resultado = await solicitarTarefa.ExecutarAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Aprova uma solicitação de obtenção de tarefa pendente, atribuindo a tarefa ao solicitante.
        /// </summary>
        [HttpPost("Solicitacoes/{id}/Aprovar")]
        [Transactional]
        public async Task<ActionResult<SolicitacaoObtencaoTarefaDTO>> AprovarSolicitacao(int id)
        {
            var resultado = await decidirTarefa.ExecutarAsync(id, true);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Recusa uma solicitação de obtenção de tarefa pendente.
        /// </summary>
        [HttpPost("Solicitacoes/{id}/Recusar")]
        [Transactional]
        public async Task<ActionResult<SolicitacaoObtencaoTarefaDTO>> RecusarSolicitacao(int id)
        {
            var resultado = await decidirTarefa.ExecutarAsync(id, false);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }
    }
}