using Application.DTO.Request;
using Application.DTO.Response;
using Application.Extensions;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Modulo:USR")]
    public class UsuarioController : ApiBaseConfigurationController<Usuario, IUsuarioService>
    {
        public UsuarioController(
            IUsuarioService usuarioService,
            IAccessorService serviceAccessor,
            Notifiable messageModel) :
            base(usuarioService, serviceAccessor, messageModel)
        { }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UsuarioRequest request)
        {
            var resultado = await _service.CriarAsync(request);
            return resultado.TeveFalha
                ? BadRequest(resultado.ObterNotificacoes())
                : Ok(resultado.Response);
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] UsuarioRequest request)
        {
            if (codigo != request.Codigo)
            {
                return BadRequest(Resultado.Falha("O código na URL não corresponde ao código no corpo da requisição.").ObterNotificacoes());
            }

            var resultado = await _service.AtualizarAsync(request);
            return resultado.TeveFalha
                ? BadRequest(resultado.ObterNotificacoes())
                : Ok(resultado.Response);
        }

        /// <summary>
        /// Remove um usuário pelo código.
        /// </summary>
        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await _service.RemoverAsync(codigo);
            return resultado.TeveFalha
                ? BadRequest(resultado.ObterNotificacoes())
                : Ok(resultado.Response);
        }

        /// <summary>
        /// Obtém todos os usuários (formato Response para o frontend).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponse>>> Get()
        {
            var usuarios = await _service.ObterTodosAsync();
            var response = usuarios.MontarUsuarioResponse();
            return Ok(response);
        }

        /// <summary>
        /// Obtém todos os usuários (formato Request/DTO).
        /// </summary>
        [HttpGet("request")]
        public async Task<ActionResult<ICollection<UsuarioRequest>>> GetRequest()
        {
            var resultado = await _service.ObterTodosRequestAsync();
            return Ok(resultado.Dado);
        }

        /// <summary>
        /// Obtém um usuário pelo código (formato DTO para compatibilidade).
        /// </summary>
        [HttpGet("{codigo}")]
        public async Task<ActionResult> Get(string codigo)
        {
            var resultado = await _service.ObterPorCodigoRequestAsync(codigo);
            var response = resultado.Dado.MontarUsuarioResponse();
            return resultado.TeveFalha
                ? NotFound(resultado.ObterNotificacoes())
                : Ok(response);
        }
    }
}