using Application.DTO.Request;
using Application.Interfaces.ApplicationInterfaces;
using Application.UseCases.Usuario;
using Domain.ApiEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcessoController : ApiBaseConfigurationController<ApiLogin, ILoginService>
    {
        private readonly IRegistroUsuarioService _registroUsuarioService;

        public AcessoController(
            Notifiable messageModel,
            ILoginService loginUserService,
            IAccessorService serviceAccessor,
            IRegistroUsuarioService registroUsuarioService)
            : base(loginUserService, messageModel, serviceAccessor)
        {
            _registroUsuarioService = registroUsuarioService;
        }

        [HttpPost(nameof(Login))]
        public async Task<ActionResult<DadosDoTokenDTO>> Login([FromBody] LoginRequestDTO loginDTO)
        {
            var dto = await _service.Autenticar(loginDTO);
            return _messageModel.Notificacoes.HasErrors() ? BadRequest(ObterNotificacoes()) : Ok(dto);
        }

        [HttpGet("RefreshToken")]
        public async Task<IActionResult> Refresh()
        {
            var cookieService = ObterService<ICookieService>();
            var dadosDeToken = await cookieService.ObterTokenRefreshTokenPorRequest(Request);
            var novoToken = await _service.RefreshAcesso(dadosDeToken);
            if (novoToken is null) return Unauthorized();

            return _messageModel.Notificacoes.HasErrors() ? BadRequest(ObterNotificacoes()) : Ok(dadosDeToken);
        }

        [HttpGet("Desconectar")]
        public async Task<ActionResult<bool>> Logout()
        {
            var usuarioCodigo = HttpContext.Request.Headers.ExtrairCodigoUsuarioDoRequest();
            var deslogado = await _service.Logout(usuarioCodigo);
            return deslogado ? Ok(deslogado) : BadRequest(deslogado);
        }

        [HttpPost(nameof(TrocarSenha))]
        public async Task<ActionResult<bool>> TrocarSenha([FromBody] LoginRequestDTO dto)
        {
            var result = await _service.TrocarSenha(dto);
            return Ok(result);
        }

        [HttpPost("Registrar")]
        public async Task<ActionResult> Post([FromBody] UsuarioRegistroRequest registroRequest)
        {
            var resultado = await _registroUsuarioService.RegistrarUsuario(registroRequest);

            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPost("ConfirmarEmail")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmarEmail([FromBody] ConfirmarEmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.usuarioCodigo) || string.IsNullOrWhiteSpace(request.token))
                return BadRequest("Usuário e Token são obrigatórios.");

            var resultado = await _registroUsuarioService.ConfirmarEmail(request.usuarioCodigo, request.token);

            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPost("SolicitarReativacao")]
        [AllowAnonymous]
        public async Task<ActionResult> SolicitarReativacao([FromBody] SolicitarReativacaoRequest request)
        {
            var useCase = ObterService<SolicitarReativacao>();
            var resultado = await useCase.ExecutarAsync(request.Email);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPost("ReativarConta")]
        [AllowAnonymous]
        public async Task<ActionResult> ReativarConta([FromBody] ReativarContaRequest request)
        {
            var useCase = ObterService<ReativarUsuario>();
            var resultado = await useCase.ExecutarAsync(request.Email, request.CodigoReativacao);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        public class ConfirmarEmailRequest
        {
            public string usuarioCodigo { get; set; }
            public string token { get; set; }
        }

        public class SolicitarReativacaoRequest
        {
            public string Email { get; set; }
        }

        public class ReativarContaRequest
        {
            public string Email { get; set; }
            public string CodigoReativacao { get; set; }
        }
    }
}