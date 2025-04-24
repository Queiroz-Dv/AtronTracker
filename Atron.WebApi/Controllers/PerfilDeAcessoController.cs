using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PerfilDeAcessoController : ApiBaseConfigurationController<PerfilDeAcessoDTO, IPerfilDeAcessoService>
    {
        public PerfilDeAcessoController(
            IPerfilDeAcessoService service,
            MessageModel messageModel) : base(service, messageModel)
        {
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            return !await _service.CriarPerfilServiceAsync(perfilDeAcessoDTO) ?
                     BadRequest(ObterNotificacoes()) :
                     Ok(ObterNotificacoes());
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            return !await _service.AtualizarPerfilServiceAsync(codigo, perfilDeAcessoDTO) ?
                     BadRequest(ObterNotificacoes()) :
                     Ok(ObterNotificacoes());
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var perfis = await _service.ObterTodosPerfisServiceAsync();
            return Ok(perfis);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult> Get(string codigo)
        {

            var perfil = await _service.ObterPerfilPorCodigoServiceAsync(codigo);
            return Ok(perfil);
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            await _service.DeletarPerfilServiceAsync(codigo);

            return _messageModel.Messages.HasErrors() ?
                    BadRequest(ObterNotificacoes()) :
                    Ok(ObterNotificacoes());
        }
    }
}
