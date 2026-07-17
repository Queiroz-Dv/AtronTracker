using Application.DTO.Request;
using Application.Interfaces.ApplicationInterfaces;
using Application.UseCases.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Extensions;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controller de acesso e autenticação de usuários.
    /// Contém endpoints para login, refresh de token, logout, troca de senha, registro e confirmação de e-mail.
    /// Os endpoints de recuperação de senha, confirmação de e-mail e reativação de conta são públicos (AllowAnonymous).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AcessoController : ControllerBase
    {
        private readonly IRegistroUsuarioService _registroUsuarioService;
        private readonly ILoginService _service;
        private readonly ICookieService _cookieService;
        private readonly SolicitarReativacao _solicitarReativacao;
        private readonly ReativarUsuario _reativarUsuario;
        private readonly ReenviarConfirmacaoEmail _reenviarConfirmacaoEmail;        

        public AcessoController(
            ILoginService loginUserService,
            IRegistroUsuarioService registroUsuarioService,
            ICookieService cookieService,
            SolicitarReativacao solicitarReativacao,
            ReativarUsuario reativarUsuario,
            ReenviarConfirmacaoEmail reenviarConfirmacaoEmail,
            ILogger<AcessoController> logger)
        {
            _registroUsuarioService = registroUsuarioService;
            _cookieService = cookieService;
            _solicitarReativacao = solicitarReativacao;
            _reativarUsuario = reativarUsuario;
            _reenviarConfirmacaoEmail = reenviarConfirmacaoEmail;
            _service = loginUserService;
        }

        /// <summary>
        /// Autentica o usuário e retorna um token JWT.
        /// </summary>
        /// <param name="loginDTO">Credenciais de acesso (código do usuário e senha).</param>
        /// <returns>200 OK com dados do token ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost(nameof(Login))]
        public async Task<ActionResult<DadosDoTokenDTO>> Login([FromBody] LoginRequestDTO loginDTO)
        {
            var resultado = await _service.Autenticar(loginDTO);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Renova o token de acesso usando o refresh token armazenado em cookie.
        /// </summary>
        /// <returns>200 OK com novos dados de token ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet("RefreshToken")]
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
        [HttpGet("Desconectar")]
        public async Task<ActionResult<bool>> Logout()
        {
            var usuarioCodigo = HttpContext.Request.Headers.ExtrairCodigoUsuarioDoRequest();
            var resultado = await _service.Logout(usuarioCodigo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Reenvia o link de confirmacao de e-mail para usuarios que ainda nao confirmaram o acesso.
        /// </summary>
        /// <param name="request">Objeto com codigo ou e-mail do usuario e a clientUri para construcao do link.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("ReenviarConfirmacaoEmail")]
        [AllowAnonymous]
        public async Task<ActionResult> ReenviarConfirmacaoEmail([FromBody] ReenviarConfirmacaoEmailRequest request)
        {
            var resultado = await _reenviarConfirmacaoEmail.ExecutarPorIdentificadorAsync(request.Identificador, request.ClientUri);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Redefine a senha do usuário usando um token de recuperação previamente solicitado.
        /// </summary>
        /// <param name="request">Objeto com o código do usuário, token de recuperação e nova senha.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost(nameof(TrocarSenha))]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> TrocarSenha([FromBody] RedefinirSenhaRequest request)
        {
            var resultado = await _registroUsuarioService.TrocarSenha(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Solicita o envio de e-mail de recuperação de senha para o usuário informado.
        /// </summary>
        /// <param name="request">Objeto com o e-mail do usuário e a clientUri para construção do link de redefinição.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("RecuperarSenha")]
        [AllowAnonymous]
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
        public async Task<ActionResult> ConfirmarEmail([FromBody] ConfirmarEmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UsuarioCodigo) || string.IsNullOrWhiteSpace(request.Identificador))
                return BadRequest("Usuario e codigo de confirmacao sao obrigatorios.");

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
        public async Task<ActionResult> ReativarConta([FromBody] ReativarContaRequest request)
        {
            var resultado = await _reativarUsuario.ExecutarAsync(request.Email, request.CodigoReativacao);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }
    }
}
