using Application.DTO;
using Application.DTO.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PerfilDeAcessoController(IPerfilDeAcessoService perfilDeAcessoService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult> Post([FromBody] PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            var resultado = await perfilDeAcessoService.CriarAsync(perfilDeAcessoDTO);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        [HttpPut("{codigo}")]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult> Put(string codigo, [FromBody] PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            if (perfilDeAcessoDTO is null)
                return BadRequest(Resultado<object>.Falha("O perfil de acesso está inválido para gravação.").Messages);

            if (codigo != perfilDeAcessoDTO.Codigo)
                return BadRequest(Resultado<object>.Falha("O código na URL não corresponde ao código no corpo da requisição.").Messages);

            var resultado = await perfilDeAcessoService.AtualizarAsync(codigo, perfilDeAcessoDTO);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        [HttpGet]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult<ICollection<PerfilDeAcessoDTO>>> Get()
        {
            var resultado = await perfilDeAcessoService.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        [HttpGet("{codigo}")]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult<PerfilDeAcessoDTO>> Get(string codigo)
        {
            var resultado = await perfilDeAcessoService.ObterPorCodigoAsync(codigo);

            return resultado.TeveFalha ?
                NotFound(resultado.Messages) :
                Ok(resultado.Dados);
        }

        [HttpDelete("{codigo}")]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await perfilDeAcessoService.RemoverAsync(codigo);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        [HttpPost("RelacionamentoPerfilUsuario")]
        [Authorize(Policy = ModuloPolicies.RelacionamentoPerfilUsuario)]
        public async Task<ActionResult> RelacionamentoPerfilUsuario([FromBody] PerfilUsuarioRequest request)
        {
            if (request is null)
                return BadRequest(Resultado<object>.Falha("O relacionamento de perfil de acesso está inválido para gravação.").Messages);

            var dto = new PerfilDeAcessoUsuarioDTO
            {
                PerfilDeAcesso = new PerfilDeAcessoDTO { Codigo = request.CodigoPerfil },
                Usuarios = (request.Usuarios ?? []).Select(usuario => new UsuarioDTO { Codigo = usuario.Codigo }).ToList()
            };

            var resultado = await perfilDeAcessoService.RelacionarPerfilDeAcessoUsuarioAsync(dto);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        [HttpGet("ObterRelacionamentoPerfilUsuario/{codigo}")]
        [Authorize(Policy = ModuloPolicies.RelacionamentoPerfilUsuario)]
        public async Task<ActionResult<PerfilDeAcessoUsuarioDTO>> ObterRelacionamentosPerfilUsuario([FromRoute] string codigo)
        {
            var resultado = await perfilDeAcessoService.ObterRelacionamentoDePerfilUsuarioPorCodigoAsync(codigo);

            return resultado.TeveFalha ?
                NotFound(resultado.Messages) :
                Ok(resultado.Dados);
        }
    }
}
