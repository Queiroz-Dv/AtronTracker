using Application.DTO.Request;
using Application.Interfaces.ApplicationInterfaces;
using Application.UseCases.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
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
        private readonly ILogger<AcessoController> _logger;

        public AcessoController(
            ILoginService loginUserService,
            IRegistroUsuarioService registroUsuarioService,
            ICookieService cookieService,
            SolicitarReativacao solicitarReativacao,
            ReativarUsuario reativarUsuario,
            ILogger<AcessoController> logger)
        {
            _registroUsuarioService = registroUsuarioService;
            _cookieService = cookieService;
            _solicitarReativacao = solicitarReativacao;
            _reativarUsuario = reativarUsuario;
            _service = loginUserService;
            _logger = logger;
        }

        [HttpPost(nameof(Login))]
        public async Task<ActionResult<DadosDoTokenDTO>> Login([FromBody] LoginRequestDTO loginDTO)
        {
            Resultado<DadosDoTokenDTO> resultado;
            try
            {
                resultado = await _service.Autenticar(loginDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha nao tratada ao autenticar o usuario {UsuarioCodigo}.", loginDTO?.CodigoDoUsuario);
                return StatusCode(500, new[] { "Falha inesperada ao autenticar o usuario." });
            }

            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpGet("RefreshToken")]
        public async Task<IActionResult> Refresh()
        {
            var dadosDoRefreshToken = await _cookieService.ObterRefreshTokenPorRequest(Request);
            var resultado = await _service.RefreshAcesso(dadosDoRefreshToken);

            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpGet("Desconectar")]
        public async Task<ActionResult<bool>> Logout()
        {
            var usuarioCodigo = HttpContext.Request.Headers.ExtrairCodigoUsuarioDoRequest();
            var resultado = await _service.Logout(usuarioCodigo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPost(nameof(TrocarSenha))]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> TrocarSenha([FromBody] RedefinirSenhaRequest request)
        {
            var resultado = await _registroUsuarioService.TrocarSenha(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPost("RecuperarSenha")]
        [AllowAnonymous]
        public async Task<ActionResult> RecuperarSenha([FromBody] SolicitarRecuperacaoSenhaRequest request)
        {
            var resultado = await _registroUsuarioService.SolicitarRecuperacaoSenha(request);
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
                return BadRequest("Usuario e Token sao obrigatorios.");

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
