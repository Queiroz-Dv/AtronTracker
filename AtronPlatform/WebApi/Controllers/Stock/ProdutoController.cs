using AtronStock.Application.DTO.Request;
using AtronStock.Application.DTO.Response;
using AtronStock.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;
using Shared.Infrastructure.Filters;

namespace AtronPlatform.WebApi.Controllers.Stock
{
    [Authorize(Policy = ModuloPolicies.Produto)]
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ProdutoController(IProdutoService service) : ControllerBase
    {
        private readonly IProdutoService _service = service;

        [HttpPost]
        [Transactional]
        public async Task<ActionResult> Post(ProdutoRequest request)
        {
            var resultado = await _service.CriarAsync(request);
            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Messages);
        }

        [HttpPost("lotes")]
        [Transactional]
        public async Task<ActionResult<SolicitacaoGeracaoProdutosLoteResponse>> PostLote(
            GerarProdutosLoteRequest request)
        {
            var resultado = await _service.SolicitarGeracaoLoteAsync(request);
            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Accepted(resultado.Dados);
        }

        [HttpPut("{codigo}")]
        [Transactional]
        public async Task<ActionResult> Put(
            string codigo,
            ProdutoAtualizacaoRequest request)
        {
            var resultado = await _service.AtualizarAsync(codigo, request);
            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Messages);
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<ProdutoResponse>>> Get()
        {
            var resultado = await _service.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<ProdutoResponse>> Get(string codigo)
        {
            var resultado = await _service.ObterPorCodigoAsync(codigo);
            return resultado.TeveFalha
                ? NotFound(resultado.Messages)
                : Ok(resultado.Dados);
        }
    }
}
