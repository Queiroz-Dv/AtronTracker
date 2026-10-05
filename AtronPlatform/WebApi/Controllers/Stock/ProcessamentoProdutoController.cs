using AtronStock.Application.DTO.Response;
using AtronStock.Application.UseCases.ProdutoCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;

namespace AtronPlatform.WebApi.Controllers.Stock;

/// <summary>
/// Controlador para gerenciamento de processamento de produtos
/// </summary>
[Authorize(Policy = ModuloPolicies.Produto)]
[ApiController]
[Route("api/[controller]")]
public sealed class ProcessamentoProdutoController(
    ObterMeusProcessamentosProdutoCase obterMeusProcessamentos,
    ObterProcessamentoProdutoCase obterProcessamento) : ControllerBase
{
    /// <summary>
    /// Obtém os processamentos de produtos do usuário logado
    /// </summary>
    /// <returns>Uma lista de processamentos de produtos</returns>
    [HttpGet]
    public async Task<ActionResult<ICollection<ProcessamentoProdutoResponse>>> Get()
    {
        var resultado = await obterMeusProcessamentos.ExecutarAsync();
        return resultado.TeveFalha
            ? BadRequest(resultado.Messages)
            : Ok(resultado.Dados);
    }

    /// <summary>
    /// Obtém um processamento de produto pelo Id
    /// </summary>
    /// <param name="id">Id do processamento de produto</param>
    /// <returns>Detalhes do processamento de produto</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProcessamentoProdutoResponse>> Get(int id)
    {
        var resultado = await obterProcessamento.ExecutarAsync(id);
        return resultado.TeveFalha
            ? NotFound(resultado.Messages)
            : Ok(resultado.Dados);
    }
}
