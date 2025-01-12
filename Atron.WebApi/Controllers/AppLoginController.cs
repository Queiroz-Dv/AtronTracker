using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppLoginController : ModuleController<ApiRegister, IRegisterUserService>
    {
        private readonly ILoginUserService _loginService;

        public AppLoginController(
            IRegisterUserService service,
            MessageModel<ApiRegister> messageModel, 
            ILoginUserService loginUserService)
            : base(service, messageModel)
        {
            _loginService = loginUserService;
        }
       
        [Route("Logar")]
        [HttpPost]
        public async Task<ActionResult<LoginDTO>> LoginUser([FromBody] LoginDTO loginDTO)
        {
            var result = await _loginService.Authenticate(loginDTO);

            return RetornoPadrao(result);
        }
      

        [Route("Registrar")]
        [HttpPost]
        public async Task<ActionResult<RegisterDTO>> RegisterUser([FromBody] RegisterDTO registerDTO)
        {
            var result = await _service.RegisterUser(registerDTO);

            return Ok(result);
        }

        [Route("Disconectar")]
        [HttpPut]
        public async Task<ActionResult> Logout()
        {
            await _loginService.Logout();
            return Ok();
        }
    }
}