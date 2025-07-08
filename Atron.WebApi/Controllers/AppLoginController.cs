using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Application.Interfaces.Contexts;
using Atron.Application.Interfaces.Services;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.DTO.API;
using Shared.DTO.API.Request;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Interfaces.Services;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    /// <summary>
    /// Controller para gerenciar o login de usuários na aplicação.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AppLoginController : ApiBaseConfigurationController<ApiLogin, ILoginService>
    {
      //  private readonly IServiceAccessor serviceAccessor;

        public AppLoginController(
            MessageModel messageModel,
            ILoginService loginUserService,
            IServiceAccessor serviceAccessor)
            : base(loginUserService, serviceAccessor ,messageModel)
        {
            //this.serviceAccessor = serviceAccessor;
        }

        /// <summary>
        /// Endpoint para logar um usuário no sistema
        /// </summary>
        /// <param name="loginDTO">DTO que será autenticado </param>
        /// <returns>O resultado do processamento</returns>
        [HttpPost]
        public async Task<ActionResult<DadosDoTokenDTO>> Login([FromBody] LoginRequestDTO loginDTO)
        {
            var dto = await _service.Autenticar(loginDTO);

            return _messageModel.Notificacoes.HasErrors() ?
             BadRequest(ObterNotificacoes()) :
             Ok(dto);
        }

        /// <summary>
        /// Endpoint para atualizar o token de acesso do usuário
        /// </summary>        
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> Refresh()
        {
            var cookieService = ObterService<ICookieService>();
            var cacheUsuarioService = ObterService<ICacheUsuarioService>();

            var dadosDeToken = cookieService.ObterTokenRefreshTokenPorRequest(Request);

            if (dadosDeToken is null)
            {
                dadosDeToken = cacheUsuarioService.ObterDadosDoTokenPorCodigoUsuario(HttpContext.Request.ExtrairCodigoUsuarioDoRequest());
            }

            var novoToken = await _service.RefreshAcesso(dadosDeToken.TokenDTO);
            if (novoToken is null) return Unauthorized();

            return _messageModel.Notificacoes.HasErrors() ?
              BadRequest(ObterNotificacoes()) :
              Ok(dadosDeToken);
        }

        /// <summary>
        /// Endpoint para desconectar o usuário do sistema
        /// </summary>
        [HttpGet("Desconectar")]
        public async Task<IActionResult> Logout()
        {
            var usuarioCodigo = HttpContext.Request.Headers.ExtrairCodigoUsuarioDoRequest();

            await _service.Logout(usuarioCodigo);
            return Ok();
        }

        /// <summary>
        /// Endpoint de trocar a senha do usuário autenticado no sistema
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("TrocarSenha")]
        public async Task<ActionResult> TrocarSenha([FromBody] LoginRequestDTO dto)
        {
            var result = await _service.TrocarSenha(dto);
            return Ok(result);
        }
    }
}