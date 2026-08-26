using AtronStock.Application.DTO.Response;
using AtronStock.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;

namespace AtronPlatform.WebApi.Controllers.Stock;

[Authorize(Policy = ModuloPolicies.Produto)]
[ApiController]
[Route("api/processamentos-produtos")]
public sealed class ProcessamentoProdutoController(
    IProcessamentoProdutoService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ICollection<ProcessamentoProdutoResponse>>> Get()
    {
        var resultado = await service.ObterMeusAsync();
        return resultado.TeveFalha
            ? BadRequest(resultado.Messages)
            : Ok(resultado.Dados);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProcessamentoProdutoResponse>> Get(int id)
    {
        var resultado = await service.ObterPorIdAsync(id);
        return resultado.TeveFalha
            ? NotFound(resultado.Messages)
            : Ok(resultado.Dados);
    }
}
