using Application.DTO.Request;
using Application.DTO.Response;
using Application.Extensions;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Authorization;

namespace AtronPlatform.WebApi.Controllers.Tracker
{
    /// <summary>
    /// Controller para gerenciamento de planejamentos de custo.
    /// Fornece endpoints para criar, atualizar, remover e consultar planejamentos de custo,
    /// bem como gerar relatórios gerais e por código nos formatos JSON e HTML para impressão.
    /// Requer autorização com a policy Modulo:PLC.
    /// </summary>
    [Authorize(Policy = ModuloPolicies.PlanejamentoCustos)]
    [ApiController]
    [Route("api/[controller]")]
    public class PlanejamentoCustoController(
        IPlanejamentoCustoService planejamentoCustoService,
        IPlanejamentoCustoRelatorioImpressaoService planejamentoCustoRelatorioImpressaoService) : ControllerBase
    {
        /// <summary>
        /// Cria um novo planejamento de custo.
        /// </summary>
        /// <param name="request">Dados do planejamento de custo a ser criado.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost]
        public async Task<ActionResult<PlanejamentoCustoResponse>> Post([FromBody] PlanejamentoCustoRequest request)
        {
            var resultado = await planejamentoCustoService.CriarAsync(request.MontarDTO());

            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Messages);
        }

        /// <summary>
        /// Obtém todos os planejamentos de custo, com filtro opcional por ano.
        /// </summary>
        /// <param name="ano">Ano de referência para filtrar os planejamentos (opcional). Se não informado, retorna todos.</param>
        /// <returns>200 OK com a lista de planejamentos de custo.</returns>
        [HttpGet]
        public async Task<ActionResult<ICollection<PlanejamentoCustoResponse>>> Get([FromQuery] int? ano)
        {
            var resultado = ano.HasValue
                ? await planejamentoCustoService.ObterPorAnoAsync(ano.Value)
                : await planejamentoCustoService.ObterTodosAsync();

            var response = resultado.Dados.Select(plc => plc.MontarResponse()).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Gera um relatório geral de planejamento de custo para o ano informado.
        /// </summary>
        /// <param name="ano">Ano de referência do relatório.</param>
        /// <returns>200 OK com o relatório geral ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("relatorio-geral")]
        public async Task<ActionResult<PlanejamentoCustoRelatorioGeralResponse>> ObterRelatorioGeral([FromQuery] int ano)
        {
            var resultado = await planejamentoCustoService.ObterRelatorioGeralAsync(ano);

            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Dados.MontarResponse());
        }

        /// <summary>
        /// Gera o relatório geral em formato HTML para impressão, para o ano informado.
        /// </summary>
        /// <param name="ano">Ano de referência do relatório.</param>
        /// <returns>200 OK com conteúdo HTML ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("relatorio-geral/impressao")]
        public async Task<ActionResult> ObterRelatorioGeralImpressao([FromQuery] int ano)
        {
            var resultado = await planejamentoCustoRelatorioImpressaoService.ObterHtmlRelatorioGeralAsync(ano);

            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Content(resultado.Dados, "text/html; charset=utf-8");
        }

        /// <summary>
        /// Obtém o relatório de um planejamento de custo específico pelo código.
        /// </summary>
        /// <param name="codigo">Código do planejamento de custo.</param>
        /// <returns>200 OK com o relatório ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("relatorio/{codigo}")]
        public async Task<ActionResult<PlanejamentoCustoRelatorioGeralResponse>> ObterRelatorioPorCodigo(string codigo)
        {
            var resultado = await planejamentoCustoService.ObterRelatorioPorCodigoAsync(codigo);

            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Dados.MontarResponse());
        }

        /// <summary>
        /// Gera o relatório em formato HTML para impressão de um planejamento de custo específico.
        /// </summary>
        /// <param name="codigo">Código do planejamento de custo.</param>
        /// <returns>200 OK com conteúdo HTML ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("relatorio/{codigo}/impressao")]
        public async Task<ActionResult> ObterRelatorioPorCodigoImpressao(string codigo)
        {
            var resultado = await planejamentoCustoRelatorioImpressaoService.ObterHtmlRelatorioPorCodigoAsync(codigo);

            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Content(resultado.Dados, "text/html; charset=utf-8");
        }

        /// <summary>
        /// Obtém os dados de um planejamento de custo pelo código.
        /// </summary>
        /// <param name="codigo">Código do planejamento de custo.</param>
        /// <returns>200 OK com os dados do planejamento ou 404 NotFound se não encontrado.</returns>
        [HttpGet("{codigo}")]
        public async Task<ActionResult<PlanejamentoCustoResponse>> Get(string codigo)
        {
            var resultado = await planejamentoCustoService.ObterPorCodigoAsync(codigo);

            return resultado.TeveFalha
                ? NotFound(resultado.Messages)
                : Ok(resultado.Dados.MontarResponse());
        }

        /// <summary>
        /// Atualiza um planejamento de custo existente pelo código.
        /// </summary>
        /// <param name="codigo">Código do planejamento de custo a ser atualizado.</param>
        /// <param name="request">Dados atualizados do planejamento de custo.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] PlanejamentoCustoRequest request)
        {
            var resultado = await planejamentoCustoService.AtualizarAsync(codigo, request.MontarDTO());

            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Messages);
        }

        /// <summary>
        /// Remove um planejamento de custo pelo código.
        /// </summary>
        /// <param name="codigo">Código do planejamento de custo a ser removido.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await planejamentoCustoService.RemoverAsync(codigo);

            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Messages);
        }
    }
}
