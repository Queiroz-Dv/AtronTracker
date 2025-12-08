using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using AtronStock.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AtronStock.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly IEstoqueService _estoqueService;

        public EstoqueController(IEstoqueService estoqueService)
        {
            _estoqueService = estoqueService;
        }

        [HttpPost("entrada")]
        public async Task<IActionResult> ProcessarEntrada([FromBody] EntradaRequest request)
        {
            var entrada = new Entrada
            {
                FornecedorId = request.FornecedorId,
                DataEntrada = DateTime.Now,
                Itens = request.Itens.Select(i => new ItemEntrada
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoCusto = i.PrecoCusto
                }).ToList()
            };

            try
            {
                await _estoqueService.ProcessarEntradaAsync(entrada);
                return Ok(new { Message = "Entrada processada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("venda")]
        public async Task<IActionResult> ProcessarVenda([FromBody] VendaRequest request)
        {
            var venda = new Venda
            {
                ClienteId = request.ClienteId,
                DataVenda = DateTime.Now,
                Itens = request.Itens.Select(i => new ItemVenda
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoVenda = i.PrecoVenda
                }).ToList()
            };

            try
            {
                await _estoqueService.ProcessarVendaAsync(venda);
                return Ok(new { Message = "Venda processada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
