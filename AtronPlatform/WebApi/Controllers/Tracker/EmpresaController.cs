using Application.DTO;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;

namespace AtronPlatform.WebApi.Controllers.Tracker
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public sealed class EmpresaController(IEmpresaService empresaService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<EmpresaDTO>> Post([FromBody] EmpresaDTO empresa)
        {
            var resultado = await empresaService.CriarAsync(empresa);
            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : CreatedAtAction(nameof(Get), new { codigo = resultado.Dados!.Codigo }, resultado.Dados);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<EmpresaDTO>>> Get()
        {
            var resultado = await empresaService.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<EmpresaDTO>> Get(string codigo)
        {
            var resultado = await empresaService.ObterPorCodigoAsync(codigo);
            return resultado.TeveFalha
                ? NotFound(resultado.Messages)
                : Ok(resultado.Dados);
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult<EmpresaDTO>> Put(string codigo, [FromBody] EmpresaDTO empresa)
        {
            if (!string.Equals(codigo, empresa.Codigo, StringComparison.OrdinalIgnoreCase))
                return BadRequest(Resultado<object>
                    .Falha(NotificacoesPadronizadas.ErroCodigoRotaDivergente)
                    .Messages);

            var resultado = await empresaService.AtualizarAsync(codigo, empresa);
            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Dados);
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await empresaService.RemoverAsync(codigo);
            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Messages);
        }
    }
}