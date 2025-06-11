using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.DTO.API;
using Shared.DTO.API.Request;
using Shared.Extensions;
using Shared.Interfaces.Handlers;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppLoginController : ApiBaseConfigurationController<ApiLogin, ILoginUserService>
    {
        private readonly ICookieHandlerService _cookieHandler;

        public AppLoginController(
            MessageModel messageModel,
            ILoginUserService loginUserService,
            ICookieHandlerService cookieHandler)
            : base(loginUserService, messageModel)
        {
            _cookieHandler = cookieHandler;
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

            return _messageModel.Messages.HasErrors() ?
             BadRequest(ObterNotificacoes()) :
             Ok();
        }

        /// <summary>
        /// Endpoint para atualizar o token de acesso do usuário
        /// </summary>        
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> Refresh()
        {
            var userInfoCookie = _cookieHandler.ExtrairTokensDoCookie(Request);
            var infoToken = await _service.RefreshAcesso(userInfoCookie);

            if (infoToken != null)
            {
                _cookieHandler.CriarCookiesDoToken(infoToken);
            }

            return _messageModel.Messages.HasErrors() ?
              BadRequest(ObterNotificacoes()) :
              Ok();
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


        [Authorize]
        [HttpGet("SessionInfo")]
        public ActionResult<DadosDoUsuario> ObterSessionInfo()
        {
            var identity = HttpContext.User;

            if (identity?.Claims != null || identity.Claims.Any())
            {
                var nomeDoUsuario = identity.FindFirst(ClaimTypes.Name)?.Value;
                var emailDoUsuario = identity.FindFirst(ClaimTypes.Email)?.Value;
                var codigoDoUsuario = identity.FindFirst(ClaimCode.CODIGO_USUARIO)?.Value;
                var codigoDoCargo = identity.FindFirst(ClaimCode.CODIGO_CARGO)?.Value;
                var codigoDoDepartamento = identity.FindFirst(ClaimCode.CODIGO_DEPARTAMENTO)?.Value;
                var codigosPerfis = identity.FindAll("perfil").Select(p => p.Value).ToList();
                var codigoModulos = identity.FindAll("modulo").Select(p => p.Value).ToList();

                if (codigoDoUsuario.IsNullOrEmpty() ||
                    nomeDoUsuario.IsNullOrEmpty() ||
                    codigoDoCargo.IsNullOrEmpty() ||
                    codigoDoDepartamento.IsNullOrEmpty() ||
                    emailDoUsuario.IsNullOrEmpty())
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(new SessionInfoDto()
                    {
                        CodigoDoCargo = codigoDoCargo,
                        CodigoDoDepartamento = codigoDoDepartamento,
                        CodigoDoUsuario = codigoDoUsuario,
                        NomeDoUsuario = nomeDoUsuario,
                        Email = emailDoUsuario,
                        CodigosPerfis = codigosPerfis,
                        ModulosCodigo = codigoModulos,
                    });
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }

    public class SessionInfoDto
    {
        public string CodigoDoUsuario { get; set; }
        public string NomeDoUsuario { get; set; }
        public string Email { get; set; }
        public string CodigoDoCargo { get; set; }
        public string CodigoDoDepartamento { get; set; }
        public List<string> CodigosPerfis { get; set; }
        public List<string> ModulosCodigo { get; set; }
    }

}