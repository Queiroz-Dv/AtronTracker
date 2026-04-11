using Application.DTO.Request;
using Application.Interfaces.ApplicationInterfaces;
using Application.UseCases.Usuario;
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
    public class AcessoController : ControllerBase
    {
        private readonly IRegistroUsuarioService _registroUsuarioService;
        private readonly ILoginService _service;
        private readonly ICookieService _cookieService;
        private readonly SolicitarReativacao _solicitarReativacao;
        private readonly ReativarUsuario _reativarUsuario;

        public AcessoController(
            ILoginService loginUserService,
            IAccessorService serviceAccessor,
            IRegistroUsuarioService registroUsuarioService,
            ICookieService cookieService,
            SolicitarReativacao solicitarReativacao,
            ReativarUsuario reativarUsuario)
        {
            _registroUsuarioService = registroUsuarioService;
            _cookieService = cookieService;
            _solicitarReativacao = solicitarReativacao;
            _reativarUsuario = reativarUsuario;
        }

        [HttpPost(nameof(Login))]
        public async Task<ActionResult<DadosDoTokenDTO>> Login([FromBody] LoginRequestDTO loginDTO)
        {
            var resultado = await _service.Autenticar(loginDTO);
            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        [HttpGet("RefreshToken")]
        public async Task<IActionResult> Refresh()
        {
            var dadosDeToken = await _cookieService.ObterTokenRefreshTokenPorRequest(Request);
            var resultado = await _service.RefreshAcesso(dadosDeToken);

            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpGet("Desconectar")]
        public async Task<ActionResult<bool>> Logout()
        {
            var usuarioCodigo = HttpContext.Request.Headers.ExtrairCodigoUsuarioDoRequest();
            var resultado = await _service.Logout(usuarioCodigo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPost(nameof(TrocarSenha))]
        public async Task<ActionResult<bool>> TrocarSenha([FromBody] LoginRequestDTO dto)
        {
            var resultado = await _registroUsuarioService.TrocarSenha(dto);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
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
            if (string.IsNullOrWhiteSpace(request.UsuarioCodigo) || string.IsNullOrWhiteSpace(request.Token))
                return BadRequest("Usuário e Token são obrigatórios.");

            var resultado = await _registroUsuarioService.ConfirmarEmail(request.UsuarioCodigo, request.Token);

            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPost("SolicitarReativacao")]
        [AllowAnonymous]
        public async Task<ActionResult> SolicitarReativacao([FromBody] SolicitarReativacaoRequest request)
        {            
            var resultado = await _solicitarReativacao.ExecutarAsync(request.Email);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPost("ReativarConta")]
        [AllowAnonymous]
        public async Task<ActionResult> ReativarConta([FromBody] ReativarContaRequest request)
        {            
            var resultado = await _reativarUsuario.ExecutarAsync(request.Email, request.CodigoReativacao);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }
    }
}