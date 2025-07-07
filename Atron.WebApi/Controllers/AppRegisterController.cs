using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppRegisterController : ApiBaseConfigurationController<ApiRegister, IRegisterUserService>
    {
        public AppRegisterController(
            IRegisterUserService service,
            MessageModel messageModel) :
            base(service, messageModel)
        { }

        [HttpPost]
        [Route("Registrar")]
        public async Task<ActionResult> Registrar([FromBody] RegisterDTO registerDTO)
        {
            await _service.RegisterUser(registerDTO);

            return _messageModel.Notificacoes.HasErrors() ?
                BadRequest(new { registrado = false }) :
                Ok(new { registrado = true });
        }


        [HttpGet]
        [Route("VerificarUsuarioPorCodigo/{codigo}")]
        public async Task<ActionResult> VerificarUsuarioPorCodigo(string codigo)
        {
            var userExist = await _service.UserExists(codigo);

            return userExist ? Ok(new RegisterDTO { Codigo = codigo }) : NotFound(null);
        }

        [HttpGet]
        [Route("VerificarEmail/{email}")]
        public async Task<ActionResult> VerificarEmail(string email)
        {
            var emailExist = await _service.EmailExists(email);
            return emailExist ? Ok(new RegisterDTO { Email = email }) : NotFound(null);
        }
    }
}