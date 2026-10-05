using Application.DTO;
using Application.DTO.Request;
using Application.Services.EntitiesServices.Tarefas;
using Application.UseCases.TarefaCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;

namespace AtronPlatform.WebApi.Controllers.Tracker
{
    /// <summary>
    /// Controller para endpoints relacionados a configurações globais e estados das tarefas.
    /// </summary>
    [Route("api/Tarefa")]
    [ApiController]
    [Authorize(Policy = ModuloPolicies.Tarefa)]
    public class TarefaConfiguracoesController(
        TarefaConfiguracoesCase tarefaConfiguracoesCase,
        TarefaEstadoService tarefaEstadoService) : ControllerBase
    {
        /// <summary>
        /// Obtém todos os estados possíveis de uma tarefa (ex: Aberta, Em andamento, Concluída).
        /// </summary>
        [HttpGet("Estados")]
        public async Task<ActionResult<IEnumerable<TarefaEstadoDTO>>> ObterEstados()
        {
            var resultado = await tarefaEstadoService.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém as configurações globais do módulo de tarefas.
        /// </summary>
        [HttpGet("Configuracoes")]
        public async Task<ActionResult<TarefaConfiguracoesDTO>> ObterConfiguracoes()
        {
            var resultado = await tarefaConfiguracoesCase.ObterAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Atualiza as configurações globais do módulo de tarefas.
        /// </summary>
        [HttpPut("Configuracoes")]
        public async Task<ActionResult> AtualizarConfiguracoes([FromBody] TarefaConfiguracoesRequest request)
        {
            var resultado = await tarefaConfiguracoesCase.AtualizarAsync(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }
    }
}