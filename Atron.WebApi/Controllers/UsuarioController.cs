using Atron.Application.DTO;
using Atron.Application.DTO.Response;
using Atron.Application.Interfaces.Services;
using Atron.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atron.Application.Extensions;
using Atron.Application.DTO.Request;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Modulo:USR")]
    public class UsuarioController : ApiBaseConfigurationController<Usuario, IUsuarioService>
    {
        public UsuarioController(
            IUsuarioService usuarioService,
            IServiceAccessor serviceAccessor,
            MessageModel messageModel) :
            base(usuarioService, serviceAccessor, messageModel)
        { }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UsuarioRequest usuarioRequest)
        {
            await _service.CriarAsync(usuarioRequest.MontarDTO());

            return _messageModel.Notificacoes.HasErrors() ?
                     BadRequest(ObterNotificacoes()) :
                     Ok(ObterNotificacoes());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponse>>> Get()
        {
            var usuarios = await _service.ObterTodosAsync();
            return Ok(usuarios.Select(usr => usr.MontarResponse()).ToList());
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult<UsuarioDTO>> Put(string codigo, [FromBody] UsuarioRequest usuario)
        {
            // Verificar o código que é enviado
            await _service.AtualizarAsync(codigo, usuario.MontarDTO());

            return _messageModel.Notificacoes.HasErrors() ?
                BadRequest(ObterNotificacoes()) : Ok(ObterNotificacoes());
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            await _service.RemoverAsync(codigo);

            return _messageModel.Notificacoes.HasErrors() ?
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