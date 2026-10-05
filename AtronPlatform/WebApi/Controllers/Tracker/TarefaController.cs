using Application.DTO;
using Application.UseCases.TarefaCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;
using Shared.Infrastructure.Filters;

namespace AtronPlatform.WebApi.Controllers.Tracker
{
    /// <summary>
    /// Controller para gerenciamento de tarefas.
    /// Contém endpoints para criação, atualização, remoção, consulta, além de fluxos de atribuição e solicitação de obtenção de tarefas.
    /// Requer autorização com a policy Modulo:TRF.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = ModuloPolicies.Tarefa)]
    public class TarefaController(
        CriarTarefaCase criarTarefa,
        AtualizarTarefaCase atualizarTarefa,
        ExcluirTarefaCase excluirTarefa,
        ObterTarefaCase obterTarefa,
        ObterMeuQuadroCase obterMeuQuadro,
        ObterEquipeCase obterEquipe,
        ObterTarefasDisponiveisCase obterDisponiveis,
        ObterAcessoTarefaCase obterAcesso,
        ObterHistoricoTarefaCase obterHistoricoTarefaCase) : ControllerBase
    {
        /// <summary>
        /// Obtém todas as tarefas cadastradas no sistema.
        /// </summary>
        /// <returns>200 OK com a lista de tarefas.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> Get()
        {
            var resultado = await obterTarefa.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém as tarefas do quadro pessoal do usuário autenticado.
        /// </summary>
        /// <returns>200 OK com a lista de tarefas do usuário ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("MeuQuadro")]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> ObterMeuQuadro()
        {
            var resultado = await obterMeuQuadro.ExecutarAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém as tarefas da equipe do usuário autenticado.
        /// </summary>
        /// <returns>200 OK com a lista de tarefas da equipe ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("Equipe")]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> ObterEquipe()
        {
            var resultado = await obterEquipe.ExecutarAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém todas as tarefas disponíveis para atribuição (sem responsável).
        /// </summary>
        /// <returns>200 OK com a lista de tarefas disponíveis ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("Disponiveis")]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> ObterDisponiveis()
        {
            var resultado = await obterDisponiveis.ExecutarAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém as capacidades do usuário autenticado nas visões de tarefas.
        /// </summary>
        /// <returns>200 OK com as capacidades ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("Acesso")]
        public async Task<ActionResult<TarefaAcessoDTO>> ObterAcesso()
        {
            var resultado = await obterAcesso.ExecutarAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }


        /// <summary>
        /// Cria uma nova tarefa no sistema.
        /// </summary>
        /// <param name="tarefa">Objeto com os dados da tarefa a ser criada.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost]
        [Transactional]
        public async Task<ActionResult> Post([FromBody] TarefaDTO tarefa)
        {
            var resultado = await criarTarefa.ExecutarAsync(tarefa);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Atualiza uma tarefa existente identificada pelo ID.
        /// </summary>
        /// <param name="id">Id da tarefa.</param>
        /// <param name="tarefa">Objeto com os dados atualizados da tarefa.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPut("{id}")]
        [Transactional]
        public async Task<ActionResult> Put(int id, [FromBody] TarefaDTO tarefa)
        {
            var resultado = await atualizarTarefa.ExecutarAsync(id, tarefa);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }


        /// <summary>
        /// Remove uma tarefa do sistema pelo seu Id.
        /// </summary>
        /// <param name="id">Id da tarefa a ser excluída.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var resultado = await excluirTarefa.ExecutarAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Obtém o histórico cronológico de movimentações da tarefa.
        /// </summary>
        /// <param name="id">Id da tarefa.</param>
        /// <returns>200 OK com as movimentações autorizadas ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("{id}/Movimentacoes")]
        public async Task<ActionResult<IReadOnlyCollection<TarefaMovimentacaoDTO>>> ObterHistorico(int id)
        {
            var resultado = await obterHistoricoTarefaCase.ExecutarAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém uma tarefa específica pelo seu Id.
        /// </summary>
        /// <param name="id">Id da tarefa.</param>
        /// <returns>200 OK com o DTO da tarefa ou 404 NotFound se não encontrada.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TarefaDTO>> Get(int id)
        {
            var resultado = await obterTarefa.ExecutarAsync(id);
            return resultado.TeveFalha ? NotFound(resultado.Messages) : Ok(resultado.Dados);
        }
    }
}