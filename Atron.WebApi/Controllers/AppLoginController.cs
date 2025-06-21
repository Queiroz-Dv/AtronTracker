using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.DTO.API.Request;
using Shared.Extensions;
using Shared.Interfaces.Handlers;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
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

        [HttpPost("TrocarSenha")]
        public async Task<ActionResult> TrocarSenha([FromBody] LoginRequestDTO dto)
        {
            var result = await _service.TrocarSenha(dto);
            return Ok(result);
        }

        /// <summary>
        /// Endpoint para logar um usuário no sistema
        /// </summary>
        /// <param name="loginDTO">DTO que será autenticado </param>
        /// <returns>O resultado do processamento</returns>
        [HttpPost("Logar")]
        public async Task<ActionResult<LoginDTO>> Logar([FromBody] LoginRequestDTO loginDTO)
        {
            var dto = await _service.Authenticate(loginDTO);

            if (dto != null)
            {
                _cookieHandler.CriarCookiesDoToken(dto.UserToken);
            }

            var tokenJson = new { token = dto.UserToken.Token, expires = dto.UserToken.Expires };


            return _messageModel.Messages.HasErrors() ?
             BadRequest(ObterNotificacoes()) :
             Ok(tokenJson);
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
            await _service.Logout();
            return Ok();
        }

        //[Authorize]
        //[HttpGet("SessionInfo")]
        //public ActionResult<DadosDoUsuario> ObterSessionInfo()
        //{
        //    var identity = HttpContext.User;

        //    if (identity?.Claims != null || identity.Claims.Any())
        //    {
        //        var nomeDoUsuario = identity.FindFirst(ClaimTypes.Name)?.Value;
        //        var emailDoUsuario = identity.FindFirst(ClaimTypes.Email)?.Value;
        //        var codigoDoUsuario = identity.FindFirst(ClaimCode.CODIGO_USUARIO)?.Value;
        //        var codigoDoCargo = identity.FindFirst(ClaimCode.CODIGO_CARGO)?.Value;
        //        var codigoDoDepartamento = identity.FindFirst(ClaimCode.CODIGO_DEPARTAMENTO)?.Value;

        //        if (codigoDoUsuario.IsNullOrEmpty() ||
        //            nomeDoUsuario.IsNullOrEmpty() ||
        //            codigoDoCargo.IsNullOrEmpty() ||
        //            codigoDoDepartamento.IsNullOrEmpty() ||
        //            emailDoUsuario.IsNullOrEmpty())
        //        {
        //            return Unauthorized();
        //        }
        //        else
        //        {
        //            var dadosDoCache = _cacheService.ObterCache<DadosDoUsuario>($"acesso:{codigoDoUsuario}");
        //            var dados = new DadosDoUsuario()
        //            {
        //                CodigoDoUsuario = codigoDoUsuario,
        //                NomeDoUsuario = nomeDoUsuario,
        //                PerfisDeAcesso = dadosDoCache.PerfisDeAcesso
        //            };

        //            return Ok(dados);
        //        }
        //    }
        //    else
        //    {
        //        return Unauthorized();
        //    }
        //}
    }
}