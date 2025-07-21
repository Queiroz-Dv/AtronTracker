using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppRegisterController : ApiBaseConfigurationController<UsuarioRegistro, IRegistroUsuarioService>
    {
        public AppRegisterController(
            IServiceAccessor serviceAccessor,
            IRegistroUsuarioService service,
            MessageModel messageModel) :
            base(service, serviceAccessor, messageModel)
        { }

        [HttpPost("Registrar")]
        public async Task<ActionResult> Registrar([FromBody] UsuarioRegistroDTO registerDTO)
        {
            await _service.RegistrarUsuario(registerDTO);

            return _messageModel.Notificacoes.HasErrors() ?
                BadRequest(ObterNotificacoes()) :
                Ok(ObterNotificacoes());
        }       
    }
}