using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppLoginController : Controller
    {
        private readonly ILoginUserService _loginService;
        private readonly IRegisterUserService _registerService;

        public AppLoginController(ILoginUserService loginUserService, IRegisterUserService registerService)
        {
            _loginService = loginUserService;
            _registerService = registerService;
        }

        [Route("Login")]
        [HttpPost]
        public async Task<ActionResult<LoginDTO>> LoginUser([FromBody] LoginDTO loginDTO)
        {
            var result = await _loginService.Authenticate(loginDTO);

            return Ok(result);
        }

        [Route("Registrar")]
        [HttpPost]
        public async Task<ActionResult<RegisterDTO>> RegisterUser([FromBody] RegisterDTO registerDTO)
        {
            var result = await _registerService.RegisterUser(registerDTO);

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> Logout([FromBody] bool disconnect)
        {
            await _loginService.Logout();
            return Ok(disconnect);
        }
    }
}