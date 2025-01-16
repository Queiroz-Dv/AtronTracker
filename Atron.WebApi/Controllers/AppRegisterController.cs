using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using Communication.Extensions;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AppRegisterController : ModuleController<ApiRegister, IRegisterUserService>
    {
        public AppRegisterController(
            IRegisterUserService service,
            MessageModel<ApiRegister> messageModel) :
            base(service, messageModel)
        { }

        [HttpPost]
        [Route("Registrar")]
        public async Task<ActionResult> Registrar([FromBody] RegisterDTO registerDTO)
        {
            await _service.RegisterUser(registerDTO);

            return _messageModel.Messages.HasErrors() ?
                BadRequest(ObterNotificacoes()) :
                Ok(ObterNotificacoes());
        }
    }
}