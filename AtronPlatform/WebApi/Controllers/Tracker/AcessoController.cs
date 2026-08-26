using Application.DTO.Request;
using Application.Interfaces.ApplicationInterfaces;
using Application.UseCases.UsuarioCases;
using AtronPlatform.WebApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;

namespace AtronPlatform.WebApi.Controllers.Tracker
{
    /// <summary>
    /// Controller de acesso e autenticação de usuários.
    /// Contém endpoints para login, refresh de token, logout, troca de senha, registro e confirmação de e-mail.
    /// Os endpoints de recuperação de senha, confirmação de e-mail e reativação de conta são públicos (AllowAnonymous).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AcessoController(
        ILoginService loginUserService,
        IRegistroUsuarioService registroUsuarioService,
        ICookieService cookieService,
        SolicitarReativacaoCase solicitarReativacao,
        ReativarUsuarioCase reativarUsuario,
        ReenviarConfirmacaoEmailCase reenviarConfirmacaoEmail) : ControllerBase
    {
        private readonly IRegistroUsuarioService _registroUsuarioService = registroUsuarioService;
        private readonly ILoginService _service = loginUserService;
        private readonly ICookieService _cookieService = cookieService;
        private readonly SolicitarReativacaoCase _solicitarReativacao = solicitarReativacao;
        private readonly ReativarUsuarioCase _reativarUsuario = reativarUsuario;
        private readonly ReenviarConfirmacaoEmailCase _reenviarConfirmacaoEmail = reenviarConfirmacaoEmail;

        /// <summary>
        /// Autentica o usuário e retorna um token JWT.
        /// </summary>
        /// <param name="loginDTO">Credenciais de acesso (código do usuário e senha).</param>
        /// <returns>200 OK com dados do token ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost(nameof(Login))]
        [EnableRateLimiting(AcessoRateLimiting.Login)]
        public async Task<ActionResult<DadosDoTokenDTO>> Login([FromBody] LoginRequestDTO loginDTO)
        {
            var resultado = await _service.Autenticar(loginDTO);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Renova o token de acesso usando o refresh token armazenado em cookie.
        /// </summary>
        /// <returns>200 OK com novos dados de token ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            var dadosDoRefreshToken = await _cookieService.ObterRefreshTokenPorRequest(Request);
            var resultado = await _service.RefreshAcesso(dadosDoRefreshToken);

            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Encerra a sessão do usuário autenticado, invalidando o token.
        /// </summary>
        /// <returns>200 OK com mensagens de confirmação ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("Desconectar")]
        [Authorize]
        public async Task<ActionResult<bool>> Logout()
        {
            var usuarioCodigo = User.FindFirst(ClaimCode.CODIGO_USUARIO)?.Value;
            if (string.IsNullOrWhiteSpace(usuarioCodigo))
                return Unauthorized();

            var resultado = await _service.Logout(usuarioCodigo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Reenvia o link de confirmacao de e-mail para usuarios que ainda nao confirmaram o acesso.
        /// </summary>
        /// <param name="request">Objeto com codigo ou e-mail do usuario.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("ReenviarConfirmacaoEmail")]
        [AllowAnonymous]
        [EnableRateLimiting(AcessoRateLimiting.ReenvioConfirmacao)]
        public async Task<ActionResult> ReenviarConfirmacaoEmail([FromBody] ReenviarConfirmacaoEmailRequest request)
        {
            var resultado = await _reenviarConfirmacaoEmail.ExecutarPorIdentificadorAsync(request.Identificador);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Redefine a senha do usuário usando um token de recuperação previamente solicitado.
        /// </summary>
        /// <param name="request">Objeto com o código do usuário, token de recuperação e nova senha.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost(nameof(TrocarSenha))]
        [AllowAnonymous]
        [EnableRateLimiting(AcessoRateLimiting.TrocaSenha)]
        public async Task<ActionResult<bool>> TrocarSenha([FromBody] RedefinirSenhaRequest request)
        {
            var resultado = await _registroUsuarioService.TrocarSenha(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Solicita o envio de e-mail de recuperação de senha para o usuário informado.
        /// </summary>
        /// <param name="request">Objeto com o e-mail ou código do usuário.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("RecuperarSenha")]
        [AllowAnonymous]
        [EnableRateLimiting(AcessoRateLimiting.RecuperacaoSenha)]
        public async Task<ActionResult> RecuperarSenha([FromBody] SolicitarRecuperacaoSenhaRequest request)
        {
            var resultado = await _registroUsuarioService.SolicitarRecuperacaoSenha(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Registra um novo usuário no sistema. Requer autorização.
        /// </summary>
        /// <param name="registroRequest">Dados do usuário a ser registrado.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("Registrar")]
        [EnableRateLimiting(AcessoRateLimiting.Registro)]
        public async Task<ActionResult> Post([FromBody] UsuarioRegistroRequest registroRequest)
        {
            var resultado = await _registroUsuarioService.RegistrarUsuario(registroRequest);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Confirma o e-mail do usuário usando o token enviado por e-mail após o registro.
        /// </summary>
        /// <param name="request">Objeto com o código do usuário e o token de confirmação.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("ConfirmarEmail")]
        [AllowAnonymous]
        [EnableRateLimiting(AcessoRateLimiting.ConfirmacaoEmail)]
        public async Task<ActionResult> ConfirmarEmail([FromBody] ConfirmarEmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UsuarioCodigo) || string.IsNullOrWhiteSpace(request.Identificador))
                return BadRequest(AuthResource.Erro_DadosConfirmacaoObrigatorios);

            var resultado = await _registroUsuarioService.ConfirmarEmail(request.UsuarioCodigo, request.Identificador);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Solicita o envio de e-mail com link para reativação de uma conta desativada.
        /// </summary>
        /// <param name="request">Objeto com o e-mail da conta a ser reativada.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("SolicitarReativacao")]
        [AllowAnonymous]
        [EnableRateLimiting(AcessoRateLimiting.Reativacao)]
        public async Task<ActionResult> SolicitarReativacao([FromBody] SolicitarReativacaoRequest request)
        {
            var resultado = await _solicitarReativacao.ExecutarAsync(request.Email);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Reativa uma conta de usuário desativada usando o código de reativação recebido por e-mail.
        /// </summary>
        /// <param name="request">Objeto com o e-mail e o código de reativação.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("ReativarConta")]
        [AllowAnonymous]
        [EnableRateLimiting(AcessoRateLimiting.Reativacao)]
        public async Task<ActionResult> ReativarConta([FromBody] ReativarContaRequest request)
        {
            var resultado = await _reativarUsuario.ExecutarAsync(request.Email, request.CodigoReativacao);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }
    }
}