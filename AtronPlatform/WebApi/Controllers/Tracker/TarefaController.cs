using Application.DTO;
using Application.DTO.Request;
using Application.Interfaces.Services;
using Application.Services.EntitiesServices.Tarefas;
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
        ITarefaService tarefaService,
        ITarefaObtencaoService tarefaObtencaoService,
        ITarefaConfiguracoesService tarefaConfiguracoesService,
        TarefaEstadoService tarefaEstadoService,
        ObterHistoricoTarefaCase obterHistoricoTarefaCase) : ControllerBase
    {
        /// <summary>
        /// Obtém todas as tarefas cadastradas no sistema.
        /// </summary>
        /// <returns>200 OK com a lista de tarefas.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> Get()
        {
            var resultado = await tarefaService.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém as tarefas do quadro pessoal do usuário autenticado.
        /// </summary>
        /// <returns>200 OK com a lista de tarefas do usuário ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("MeuQuadro")]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> ObterMeuQuadro()
        {
            var resultado = await tarefaObtencaoService.ObterMeuQuadroAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém as tarefas da equipe do usuário autenticado.
        /// </summary>
        /// <returns>200 OK com a lista de tarefas da equipe ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("Equipe")]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> ObterEquipe()
        {
            var resultado = await tarefaObtencaoService.ObterEquipeAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém todas as tarefas disponíveis para atribuição (sem responsável).
        /// </summary>
        /// <returns>200 OK com a lista de tarefas disponíveis ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("Disponiveis")]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> ObterDisponiveis()
        {
            var resultado = await tarefaObtencaoService.ObterDisponiveisAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém as capacidades do usuário autenticado nas visões de tarefas.
        /// </summary>
        /// <returns>200 OK com as capacidades ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("Acesso")]
        public async Task<ActionResult<TarefaAcessoDTO>> ObterAcesso()
        {
            var resultado = await tarefaObtencaoService.ObterAcessoAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém todas as solicitações de obtenção de tarefas pendentes de aprovação.
        /// </summary>
        /// <returns>200 OK com a lista de solicitações ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("Solicitacoes")]
        public async Task<ActionResult<IEnumerable<SolicitacaoObtencaoTarefaDTO>>> ObterSolicitacoes()
        {
            var resultado = await tarefaObtencaoService.ObterSolicitacoesAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém todos os estados possíveis de uma tarefa (ex: Aberta, Em andamento, Concluída).
        /// </summary>
        /// <returns>200 OK com a lista de estados.</returns>
        [HttpGet("Estados")]
        public async Task<ActionResult<IEnumerable<TarefaEstadoDTO>>> ObterEstados()
        {
            var resultado = await tarefaEstadoService.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém as configurações globais do módulo de tarefas.
        /// </summary>
        /// <returns>200 OK com as configurações ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("Configuracoes")]
        public async Task<ActionResult<TarefaConfiguracoesDTO>> ObterConfiguracoes()
        {
            var resultado = await tarefaConfiguracoesService.ObterAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Atualiza as configurações globais do módulo de tarefas.
        /// </summary>
        /// <param name="request">Objeto com as novas configurações a serem aplicadas.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPut("Configuracoes")]
        public async Task<ActionResult> AtualizarConfiguracoes([FromBody] TarefaConfiguracoesRequest request)
        {
            var resultado = await tarefaConfiguracoesService.AtualizarAsync(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
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
            var resultado = await tarefaService.CriarAsync(tarefa);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Atualiza uma tarefa existente identificada pelo ID.
        /// </summary>
        /// <param name="id">Identificador numérico da tarefa.</param>
        /// <param name="tarefa">Objeto com os dados atualizados da tarefa.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPut("{id}")]
        [Transactional]
        public async Task<ActionResult> Put(int id, [FromBody] TarefaDTO tarefa)
        {
            var resultado = await tarefaService.AtualizarAsync(id, tarefa);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Atribui a tarefa diretamente ao usuário autenticado (assumir tarefa disponível).
        /// </summary>
        /// <param name="id">Identificador numérico da tarefa a ser assumida.</param>
        /// <returns>200 OK com o DTO da tarefa atualizada ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("{id}/Assumir")]
        [Transactional]
        public async Task<ActionResult<TarefaDTO>> Assumir(int id)
        {
            var resultado = await tarefaObtencaoService.AssumirAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Cria uma solicitação para que o usuário autenticado possa obter a tarefa (requer aprovação).
        /// </summary>
        /// <param name="id">Identificador numérico da tarefa para a qual se está solicitando obtenção.</param>
        /// <returns>200 OK com o DTO da solicitação ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("{id}/SolicitarObtencao")]
        [Transactional]
        public async Task<ActionResult<SolicitacaoObtencaoTarefaDTO>> SolicitarObtencao(int id)
        {
            var resultado = await tarefaObtencaoService.SolicitarAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Aprova uma solicitação de obtenção de tarefa pendente, atribuindo a tarefa ao solicitante.
        /// </summary>
        /// <param name="id">Identificador numérico da solicitação a ser aprovada.</param>
        /// <returns>200 OK com o DTO da solicitação atualizada ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("Solicitacoes/{id}/Aprovar")]
        [Transactional]
        public async Task<ActionResult<SolicitacaoObtencaoTarefaDTO>> AprovarSolicitacao(int id)
        {
            var resultado = await tarefaObtencaoService.DecidirAsync(id, true);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Recusa uma solicitação de obtenção de tarefa pendente.
        /// </summary>
        /// <param name="id">Identificador numérico da solicitação a ser recusada.</param>
        /// <returns>200 OK com o DTO da solicitação atualizada ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("Solicitacoes/{id}/Recusar")]
        [Transactional]
        public async Task<ActionResult<SolicitacaoObtencaoTarefaDTO>> RecusarSolicitacao(int id)
        {
            var resultado = await tarefaObtencaoService.DecidirAsync(id, false);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Remove uma tarefa do sistema pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador da tarefa a ser excluída.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var resultado = await tarefaService.ExcluirAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Obtém o histórico cronológico de movimentações da tarefa.
        /// </summary>
        /// <param name="id">Identificador numérico da tarefa.</param>
        /// <returns>200 OK com as movimentações autorizadas ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("{id}/Movimentacoes")]
        public async Task<ActionResult<IReadOnlyCollection<TarefaMovimentacaoDTO>>> ObterHistorico(int id)
        {
            var resultado = await obterHistoricoTarefaCase.ExecutarAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém uma tarefa específica pelo seu identificador numérico.
        /// </summary>
        /// <param name="id">Identificador numérico da tarefa.</param>
        /// <returns>200 OK com o DTO da tarefa ou 404 NotFound se não encontrada.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TarefaDTO>> Get(int id)
        {
            var resultado = await tarefaService.ObterPorId(id);
            return resultado.TeveFalha ? NotFound(resultado.Messages) : Ok(resultado.Dados);
        }
    }
}
