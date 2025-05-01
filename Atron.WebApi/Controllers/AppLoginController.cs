using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Application.Interfaces;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.DTO.API;
using Shared.DTO.API.Request;
using Shared.Extensions;
using Shared.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppLoginController : ApiBaseConfigurationController<ApiLogin, ILoginUserService>
    {
        private readonly IUsuarioService _usuarioService;
        public AppLoginController(
            IUsuarioService usuarioService,
            MessageModel messageModel,
            ILoginUserService loginUserService)
            : base(loginUserService, messageModel)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Endpoint para logar um usuário no sistema
        /// </summary>
        /// <param name="loginDTO">DTO que será autenticado </param>
        /// <returns>O resultado do processamento</returns>
        [HttpPost]
        [Route("Logar")]
        public async Task<ActionResult<LoginDTO>> Logar([FromBody] LoginRequestDTO loginDTO)
        {
            var result = await _service.Authenticate(loginDTO);
            if (!result.Authenticated)
            {
                return Unauthorized(ObterNotificacoes());
            }

            Response.Headers.Add("Authorization", $"Bearer {result.UserToken.Token}");

            // Replace Json.Convert with JsonSerializer.Serialize
            return Ok(result);
        }

        [Route("Desconectar")]
        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            await _service.Logout();
            return Ok();
        }


        [Route("ObterUsuarioLogado")]
        [HttpGet]
        public async Task<ActionResult<DadosDoUsuario>> UsuarioLogado()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity?.Claims != null || identity.Claims.Any())
            {
                var nomeDoUsuario = identity.FindFirst(ClaimTypes.Name)?.Value;
                var emailDoUsuario = identity.FindFirst(ClaimTypes.Email)?.Value;
                var tempoDoToken = Convert.ToDateTime(identity.FindFirst(ClaimTypes.Expiration)?.Value);
                var codigoDoUsuario = identity.FindFirst(ClaimCode.CODIGO_USUARIO)?.Value;
                var codigoDoCargo = identity.FindFirst(ClaimCode.CODIGO_CARGO)?.Value;
                var codigoDoDepartamento = identity.FindFirst(ClaimCode.CODIGO_DEPARTAMENTO)?.Value;

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
                    return Ok(new DadosDoUsuario()
                    {
                        CodigoDoCargo = codigoDoCargo,
                        CodigoDoDepartamento = codigoDoDepartamento,
                        CodigoDoUsuario = codigoDoUsuario,
                        NomeDoUsuario = nomeDoUsuario,
                        Email = emailDoUsuario,
                        Expiracao = tempoDoToken
                    });
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}