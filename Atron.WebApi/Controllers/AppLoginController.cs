using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.DTO.API;
using Shared.DTO.API.Request;
using Shared.Extensions;
using Shared.Interfaces.Handlers;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    /// <summary>
    /// Controller para gerenciar o login de usuários na aplicação.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AppLoginController : ApiBaseConfigurationController<ApiLogin, ILoginUserService>
    {
        private readonly ICookieHandlerService _cookieHandler;
        private readonly ICacheHandlerService _cacheHandler;

        public AppLoginController(
            MessageModel messageModel,
            ILoginUserService loginUserService,
            ICookieHandlerService cookieHandler,
            ICacheHandlerService cacheHandler)
            : base(loginUserService, messageModel)
        {
            _cookieHandler = cookieHandler;
            _cacheHandler = cacheHandler;
        }

        /// <summary>
        /// Endpoint para logar um usuário no sistema
        /// </summary>
        /// <param name="loginDTO">DTO que será autenticado </param>
        /// <returns>O resultado do processamento</returns>
        [HttpPost]
        public async Task<ActionResult<LoginDTO>> Login([FromBody] LoginRequestDTO loginDTO)
        {
            var dto = await _service.Authenticate(loginDTO);

            if (dto != null)
            {
                _cookieHandler.CriarCookiesDoToken(dto.UserToken);
            }      
            
            return _messageModel.Messages.HasErrors() ?
             BadRequest(ObterNotificacoes()) :
             Ok(new InfoToken(dto.UserToken.Token, dto.UserToken.Expires));
        }

        /// <summary>
        /// Endpoint para atualizar o token de acesso do usuário
        /// </summary>        
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> Refresh()
        {
            var cookie = _cookieHandler.ExtrairInfoTokensDoCookie(Request);

            if (cookie is null)
            {
                cookie = _cacheHandler.ObterInfoTokensDoCachePorRequest(Request);
            }

            var novoToken = await _service.RefreshAcesso(cookie);

            if (novoToken is null) return Unauthorized();

            var token = novoToken.Token;
            var expires = novoToken.Expires;

            _cookieHandler.CriarCookiesDoToken(novoToken);

            return _messageModel.Messages.HasErrors() ?
              BadRequest(ObterNotificacoes()) :
              Ok(new { token, expires });
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