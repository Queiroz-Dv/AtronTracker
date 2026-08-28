using Application.DTO.Request;
using Application.DTO.Response;
using Application.UseCases.EmpresaCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtronPlatform.WebApi.Controllers.Tracker
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public sealed class EmpresaController(CadastrarEmpresaCase cadastrar, ObterEmpresaCase obterEmpresa) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<EmpresaResponse>> Post(EmpresaCadastroRequest request)
        {
            var resultado = await cadastrar.ExecutarAsync(request);
            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : CreatedAtAction(nameof(Get), resultado.Dados);
        }

        [HttpGet]
        public async Task<ActionResult<EmpresaResponse>> Get()
        {
            var resultado = await obterEmpresa.ExecutarAsync();
            if (resultado.TeveFalha)
                return BadRequest(resultado.Messages);
            return resultado.Dados is null ? NoContent() : Ok(resultado.Dados);
        }
    }
}

