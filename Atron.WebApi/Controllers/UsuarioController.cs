using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Extensions;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuarioController : ModuleController<Usuario, IUsuarioService>
    {

        public UsuarioController(IUsuarioService usuarioService,
            MessageModel messageModel) :
            base(usuarioService, messageModel)
        { }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UsuarioDTO usuario)
        {
            await _service.CriarAsync(usuario);

            return _messageModel.Messages.HasErrors() ?
                     BadRequest(ObterNotificacoes()) :
                     Ok(ObterNotificacoes());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> Get()
        {
            var usuarios = await _service.ObterTodosAsync();
            return Ok(usuarios);
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult<UsuarioDTO>> Put(string codigo, [FromBody] UsuarioDTO usuario)
        {
            // Verificar o código que é enviado
            await _service.AtualizarAsync(usuario);

            return _messageModel.Messages.HasErrors() ?
                BadRequest(ObterNotificacoes()) : Ok(ObterNotificacoes());
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            await _service.RemoverAsync(codigo);

            return _messageModel.Messages.HasErrors() ?
                BadRequest(ObterNotificacoes()) :
                Ok(ObterNotificacoes());
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<UsuarioDTO>> Get(string codigo)
        {
            var usuario = await _service.ObterPorCodigoAsync(codigo);
            return usuario is null ?
             NotFound(ObterNotificacoes()) :
             Ok(usuario);
        }
    }
}