using Application.DTO.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.Filters;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize(Policy = "Modulo:USR")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController(IUsuarioService usuarioService) : ControllerBase
    {
        [HttpPost]
        [Transactional]
        public async Task<ActionResult> Post([FromBody] UsuarioRequest request)
        {
            var resultado = await usuarioService.CriarAsync(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPut("{codigo}")]
        [Transactional]
        public async Task<ActionResult> Put(string codigo, [FromBody] UsuarioRequest request)
        {
            if (codigo != request.Codigo)
                return BadRequest(Resultado<object>.Falha("O código na URL não corresponde ao código no corpo da requisição.").Messages);

            var resultado = await usuarioService.AtualizarAsync(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpDelete("{codigo}")]
        [Transactional]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await usuarioService.RemoverAsync(codigo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPut("desativar/{codigo}")]
        [Transactional]
        public async Task<ActionResult> Desativar(string codigo)
        {
            var resultado = await usuarioService.DesativarAsync(codigo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var resultado = await usuarioService.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult> Get(string codigo)
        {
            var resultado = await usuarioService.ObterPorCodigoAsync(codigo);
            return resultado.TeveFalha ? NotFound(resultado.Messages) : Ok(resultado.Dados);
        }
    }
}