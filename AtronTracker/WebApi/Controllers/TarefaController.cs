using Application.DTO;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = ModuloPolicies.Tarefa)]
    public class TarefaController(ITarefaService tarefaService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> Get()
        {
            var resultado = await tarefaService.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        [HttpGet("MeuQuadro")]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> ObterMeuQuadro()
        {
            var resultado = await tarefaService.ObterMeuQuadroAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpGet("Equipe")]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> ObterEquipe()
        {
            var resultado = await tarefaService.ObterEquipeAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpGet("Disponiveis")]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> ObterDisponiveis()
        {
            var resultado = await tarefaService.ObterDisponiveisAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpGet("Solicitacoes")]
        public async Task<ActionResult<IEnumerable<SolicitacaoObtencaoTarefaDTO>>> ObterSolicitacoes()
        {
            var resultado = await tarefaService.ObterSolicitacoesAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpGet("Estados")]
        public async Task<ActionResult<IEnumerable<TarefaEstadoDTO>>> ObterEstados()
        {
            var resultado = await tarefaService.ObterEstadosAsync();
            return Ok(resultado.Dados);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TarefaDTO tarefa)
        {
            var resultado = await tarefaService.CriarAsync(tarefa);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] TarefaDTO tarefa)
        {
            var resultado = await tarefaService.AtualizarAsync(id, tarefa);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPost("{id}/Assumir")]
        public async Task<ActionResult<TarefaDTO>> Assumir(int id)
        {
            var resultado = await tarefaService.AssumirAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpPost("{id}/SolicitarObtencao")]
        public async Task<ActionResult<SolicitacaoObtencaoTarefaDTO>> SolicitarObtencao(int id)
        {
            var resultado = await tarefaService.SolicitarObtencaoAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpPost("Solicitacoes/{id}/Aprovar")]
        public async Task<ActionResult<SolicitacaoObtencaoTarefaDTO>> AprovarSolicitacao(int id)
        {
            var resultado = await tarefaService.AprovarSolicitacaoAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpPost("Solicitacoes/{id}/Recusar")]
        public async Task<ActionResult<SolicitacaoObtencaoTarefaDTO>> RecusarSolicitacao(int id)
        {
            var resultado = await tarefaService.RecusarSolicitacaoAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var resultado = await tarefaService.ExcluirAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TarefaDTO>> Get(int id)
        {
            var resultado = await tarefaService.ObterPorId(id);
            return resultado.TeveFalha ? NotFound(resultado.Messages) : Ok(resultado.Dados);
        }
    }
}
