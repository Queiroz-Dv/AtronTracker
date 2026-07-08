using Application.DTO.Request;
using Application.Interfaces.ApplicationInterfaces;
using Application.UseCases.Usuario;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controller responsável por operações de autenticação e registro de usuários.
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
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AcessoController> _logger;

        public AcessoController(
            ILoginService loginUserService,
            IAccessorService serviceAccessor,
            IRegistroUsuarioService registroUsuarioService,
            ICookieService cookieService,
            SolicitarReativacao solicitarReativacao,
            ReativarUsuario reativarUsuario,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            ILogger<AcessoController> logger)
        {
            _registroUsuarioService = registroUsuarioService;
            _cookieService = cookieService;
            _solicitarReativacao = solicitarReativacao;
            _reativarUsuario = reativarUsuario;
            _service = loginUserService;
            _configuration = configuration;
            _environment = environment;
            _logger = logger;
        }

        /// <summary>
        /// Autentica um usuário e retorna os dados do token (access + refresh).
        /// </summary>
        /// <param name="loginDTO">Credenciais do usuário (código e senha).</param>
        /// <returns>Dados do token em caso de sucesso; mensagens de erro em caso de falha.</returns>
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

                if (ExibirDetalhesDiagnostico())
                    return StatusCode(500, LoginErroDiagnosticoResponse.FromException(ex));

                return StatusCode(500, new[] { "Falha inesperada ao autenticar o usuario." });
            }

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Dados);
        }

        /// <summary>
        /// Atualiza o token de acesso usando o refresh token presente no cookie da requisição.
        /// </summary>
        /// <returns>Novo token de acesso em caso de sucesso; mensagens em caso de falha.</returns>
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> Refresh()
        {
            var dadosDoRefreshToken = await _cookieService.ObterRefreshTokenPorRequest(Request);
            var resultado = await _service.RefreshAcesso(dadosDoRefreshToken);

            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Encerra a sessão do usuário atual, removendo cookies e invalidando refresh token.
        /// </summary>
        /// <returns>Mensagens de sucesso ou falha.</returns>
        [HttpGet("Desconectar")]
        public async Task<ActionResult<bool>> Logout()
        {
            var usuarioCodigo = HttpContext.Request.Headers.ExtrairCodigoUsuarioDoRequest();
            var resultado = await _service.Logout(usuarioCodigo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        ///Atualiza a senha do usuário utilizando o token de recuperação.
        /// </summary>
        /// <param name="request">DTO contendo token, código e senhas criptografadas.</param>
        /// <returns>Mensagens de sucesso ou falha.</returns>
        [HttpPost(nameof(TrocarSenha))]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> TrocarSenha([FromBody] RedefinirSenhaRequest request)
        {
            var resultado = await _registroUsuarioService.TrocarSenha(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Solicitar Recuperação de Senha.
        /// </summary>
        /// <param name="request">Email ou Codigo para envio do token de alteração de senha.</param>
        /// <returns>Ok ou BadRequest</returns>
        [HttpPost("RecuperarSenha")]
        [AllowAnonymous]
        public async Task<ActionResult> RecuperarSenha([FromBody] SolicitarRecuperacaoSenhaRequest request)
        {
            var resultado = await _registroUsuarioService.SolicitarRecuperacaoSenha(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Registra um novo usuário no sistema e envia e-mail de confirmação.
        /// </summary>
        /// <param name="registroRequest">Dados necessários para registro (código, nome, e-mail, senha, etc.).</param>
        /// <returns>Mensagens de sucesso ou falha.</returns>
        [HttpPost("Registrar")]
        public async Task<ActionResult> Post([FromBody] UsuarioRegistroRequest registroRequest)
        {
            var resultado = await _registroUsuarioService.RegistrarUsuario(registroRequest);

            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Confirma o e-mail do usuário usando o código do usuário e o token de confirmação.
        /// </summary>
        /// <param name="request">Objeto contendo UsuarioCodigo e Token.</param>
        /// <returns>Mensagens de sucesso ou falha.</returns>
        [HttpPost("ConfirmarEmail")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmarEmail([FromBody] ConfirmarEmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UsuarioCodigo) || string.IsNullOrWhiteSpace(request.Token))
                return BadRequest("Usuário e Token são obrigatórios.");

            var resultado = await _registroUsuarioService.ConfirmarEmail(request.UsuarioCodigo, request.Token);

            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Solicita reativação de conta — inicia fluxo de reativação (envio de código por e-mail).
        /// </summary>
        /// <param name="request">Objeto contendo o e-mail para solicitação de reativação.</param>
        /// <returns>Mensagens de sucesso ou falha.</returns>
        [HttpPost("SolicitarReativacao")]
        [AllowAnonymous]
        public async Task<ActionResult> SolicitarReativacao([FromBody] SolicitarReativacaoRequest request)
        {
            var resultado = await _solicitarReativacao.ExecutarAsync(request.Email);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Reativa uma conta usando o código de reativação enviado por e-mail.
        /// </summary>
        /// <param name="request">Objeto contendo e-mail e código de reativação.</param>
        /// <returns>Mensagens de sucesso ou falha.</returns>
        [HttpPost("ReativarConta")]
        [AllowAnonymous]
        public async Task<ActionResult> ReativarConta([FromBody] ReativarContaRequest request)
        {
            var resultado = await _reativarUsuario.ExecutarAsync(request.Email, request.CodigoReativacao);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        private bool ExibirDetalhesDiagnostico()
        {
            return _environment.IsDevelopment()
                || _configuration.GetValue<bool>("Diagnostico:Habilitado")
                || _configuration.GetValue<bool>("ConexaoBancoTeste:Habilitado");
        }
    }

    public class LoginErroDiagnosticoResponse
    {
        public string Tipo { get; set; }
        public string Mensagem { get; set; }
        public string InnerTipo { get; set; }
        public string InnerMensagem { get; set; }

        public static LoginErroDiagnosticoResponse FromException(Exception ex)
        {
            return new LoginErroDiagnosticoResponse
            {
                Tipo = ex.GetType().FullName,
                Mensagem = ex.Message,
                InnerTipo = ex.InnerException?.GetType().FullName,
                InnerMensagem = ex.InnerException?.Message
            };
        }
    }
}
