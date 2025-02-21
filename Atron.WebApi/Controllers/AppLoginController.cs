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
    public class AppLoginController : ModuleController<ApiLogin, ILoginUserService>
    {
        public AppLoginController(
            MessageModel messageModel,
            ILoginUserService loginUserService)
            : base(loginUserService, messageModel)
        { }

        /// <summary>
        /// Endpoint para logar um usuário no sistema
        /// </summary>
        /// <param name="loginDTO">DTO que será autenticado </param>
        /// <returns>O resultado do processamento</returns>
        [HttpPost]
        [Route("Logar")]
        public async Task<ActionResult<LoginDTO>> Logar([FromBody] LoginDTO loginDTO)
        {
            var result = await _service.Authenticate(loginDTO);
            if (!result.Authenticated)
            {
                return Unauthorized(ObterNotificacoes());
            }

            Response.Headers.Add("Authorization", $"Bearer {result.UserToken.Token}");

            return Ok(result);
        }

        [Route("Disconectar")]
        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            await _service.Logout();
            return Ok();
        }
    }
}