using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Filters;

namespace AtronStock.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpPost]
        [Transactional]
        public async Task<IActionResult> Post([FromBody] ProdutoRequest request)
        {
            var resultado = await _produtoService.RegistrarProdutoAsync(request);

            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        //[HttpPost("lote")]
        //public async Task<IActionResult> RegistrarProdutosEmLote([FromBody] ProdutoBulkRequest request)
        //{
        //    var produtoBase = new Produto
        //    {
        //        Codigo = request.Codigo,
        //        Descricao = request.Descricao,
        //        Categorias = request.CategoriaCodigos.Select(codigo => new ProdutoCategoria { CategoriaCodigo = codigo }).ToList()
        //    };

        //    await _produtoService.RegistrarProdutosEmLoteAsync(produtoBase, request.Quantidade);
        //    return Ok(new { Message = $"{request.Quantidade} produtos registrados com sucesso." });
        //}

        //[HttpPut("inativar/{id}")]
        //public async Task<IActionResult> InativarProduto(int id)
        //{
        //    try
        //    {
        //        await _produtoService.InativarProdutoAsync(id);
        //        return Ok(new { Message = "Produto inativado com sucesso." });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(new { Error = ex.Message });
        //    }
        //}

        //[HttpGet]
        //public async Task<ActionResult<ICollection<Produto>>> Get()
        //{
        //    var resultado = await _produtoService.ObterTodos();
        //    return Ok(resultado.Dado);
        //}


        //[HttpPut("inativar-lote")]
        //public async Task<IActionResult> InativarProdutosEmLote([FromBody] List<int> ids)
        //{
        //    try
        //    {
        //        await _produtoService.InativarProdutosEmLoteAsync(ids);
        //        return Ok(new { Message = "Produtos inativados com sucesso." });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(new { Error = ex.Message });
        //    }
        //}
    }
}
