using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Notification.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [Route("CriarUsuario")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UsuarioDTO usuario)
        {
            if (usuario is null)
            {
                return BadRequest(new NotificationMessage("Registro inválido, tente novamente"));
            }

            await _usuarioService.CriarAsync(usuario);

            return Ok(_usuarioService.notificationMessages);
        }

        [Route("ObterUsuários")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> Get()
        {
            var usuarios = await _usuarioService.ObterTodosAsync();

            if (usuarios is null)
            {
                return NotFound("Não foi encontrado nenhum registro");
            }

            return Ok(usuarios);
        }

        [HttpPut("AtualizarUsuario/{codigo}")]
        public async Task<ActionResult<UsuarioDTO>> Put(string codigo, [FromBody] UsuarioDTO usuario)
        {
            if (!string.IsNullOrEmpty(codigo))
            {
                await _usuarioService.AtualizarAsync(usuario);

                if (_usuarioService.notificationMessages.HasErrors())
                {
                    foreach (var item in _usuarioService.notificationMessages)
                    {
                        return BadRequest(item.Message);
                    }
                }

                return Ok(_usuarioService.notificationMessages);
            }

            return BadRequest();
        }

        [HttpDelete("ExcluirUsuario")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var usuario = await _usuarioService.ObterPorCodigoAsync(codigo);

            if (usuario is null)
            {
                return NotFound(new NotificationMessage("Usuário não encontrado"));
            }

            await _usuarioService.RemoverAsync(usuario.Id);

            if (_usuarioService.notificationMessages.HasErrors())
            {
                return BadRequest(_usuarioService.notificationMessages);
            }

            return Ok(_usuarioService.notificationMessages.First().Message);
        }
    }
}