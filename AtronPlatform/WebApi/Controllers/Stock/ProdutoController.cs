using AtronStock.Application.DTO.Request;
using AtronStock.Application.DTO.Response;
using AtronStock.Application.UseCases.ProdutoCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;

namespace AtronPlatform.WebApi.Controllers.Stock
{
    /// <summary>
    /// Controlador para gerenciar entidades de Produto.
    /// </summary>
    [Authorize(Policy = ModuloPolicies.Produto)]
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ProdutoController(
        CriarProdutoCase criarProduto,
        SolicitarGeracaoProdutosLoteCase solicitarGeracaoLote,
        AtualizarProdutoCase atualizarProduto,
        ObterProdutoCase obterProduto) : ControllerBase
    {
        /// <summary>
        /// Cria um novo produto.
        /// </summary>
        /// <param name="request">Dados do produto a ser criado.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPost]
        public async Task<ActionResult> Post(ProdutoRequest request)
        {
            var resultado = await criarProduto.ExecutarAsync(request);
            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Messages);
        }

        /// <summary>
        /// Solicita a geração em lote de produtos.
        /// </summary>
        /// <param name="request">Dados para geração em lote.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPost("lotes")]
        public async Task<ActionResult<SolicitacaoGeracaoProdutosLoteResponse>> PostLote(
            GerarProdutosLoteRequest request)
        {
            var resultado = await solicitarGeracaoLote.ExecutarAsync(request);
            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Accepted(resultado.Dados);
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="codigo">Código do produto a ser atualizado.</param>
        /// <param name="request">Dados atualizados do produto.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(
            string codigo,
            ProdutoAtualizacaoRequest request)
        {
            var resultado = await atualizarProduto.ExecutarAsync(codigo, request);
            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Messages);
        }

        /// <summary>
        /// Obtém todos os produtos.
        /// </summary>
        /// <returns>Lista de produtos.</returns>
        [HttpGet]
        public async Task<ActionResult<ICollection<ProdutoResponse>>> Get()
        {
            var resultado = await obterProduto.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém um produto pelo código.
        /// </summary>
        /// <param name="codigo">Código do produto.</param>
        /// <returns>Dados do produto.</returns>
        [HttpGet("{codigo}")]
        public async Task<ActionResult<ProdutoResponse>> Get(string codigo)
        {
            var resultado = await obterProduto.ObterPorCodigoAsync(codigo);
            return resultado.TeveFalha
                ? NotFound(resultado.Messages)
                : Ok(resultado.Dados);
        }
    }
}
