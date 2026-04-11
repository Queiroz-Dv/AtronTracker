using Application.Interfaces.ApplicationInterfaces;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.Services.AuthServices
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly IDadosComplementaresDoUsuarioService _dadosComplementaresDoUsuarioService;
        private readonly ITokenService _tokenService;
        private readonly ICacheUsuarioService _cacheUsuarioService;
        private readonly ICookieService _cookieService;
        private readonly IUserIdentityService _userIdentityService;
        private readonly IValidador<DadosDoTokenDTO> _tokenValidator;

        private const string ERRO_AUTENTICACAO = "Erro ao autenticar usuário. Verifique as informações e tente novamente.";

        public LoginService(
            ILoginRepository loginRepository,
            IUsuarioService usuarioService,
            IDadosComplementaresDoUsuarioService dadosComplementaresDoUsuarioService,
            ITokenService tokenService,
            ICacheUsuarioService cacheUsuarioService,
            ICookieService cookieService,
            IUserIdentityService userIdentityService,
            IValidador<DadosDoTokenDTO> tokenValidator)
        {
            _loginRepository = loginRepository;
            _usuarioService = usuarioService;
            _dadosComplementaresDoUsuarioService = dadosComplementaresDoUsuarioService;
            _tokenService = tokenService;
            _cacheUsuarioService = cacheUsuarioService;
            _cookieService = cookieService;
            _userIdentityService = userIdentityService;
            _tokenValidator = tokenValidator;
        }

        public async Task<Resultado<DadosDoTokenDTO>> Autenticar(LoginRequestDTO loginRequest)
        {
            var resultadoUsuario = await _usuarioService.ObterPorCodigoAsync(loginRequest.CodigoDoUsuario);

            if (resultadoUsuario?.Dados == null)
                return Resultado<DadosDoTokenDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dadosComplementares = await _dadosComplementaresDoUsuarioService
                .ObterInformacoesComplementaresDoUsuario(resultadoUsuario.Dados);

            var dadosDoToken = await _tokenService.ObterTokenComRefreshToken(dadosComplementares);

            var usuarioAutenticado = await _loginRepository.AutenticarUsuarioAsync(new UsuarioIdentity
            {
                Codigo = dadosComplementares.DadosDoUsuario.CodigoDoUsuario,
                Token = dadosDoToken.TokenDTO.Token,
                RefreshToken = dadosDoToken.RefrehTokenDTO.Token,
                RefreshTokenExpireTime = dadosDoToken.RefrehTokenDTO.Expires,
                Senha = loginRequest.Senha
            });

            if (!usuarioAutenticado)
                return Resultado<DadosDoTokenDTO>.Falha(ERRO_AUTENTICACAO);

            var token = new DadosDoTokenDTO(dadosDoToken.TokenDTO.Token, dadosDoToken.TokenDTO.Expires);

            _cacheUsuarioService.GravarCacheDeAcessoTokenInfo(dadosComplementares, dadosDoToken);
            _cookieService.CriarCookieDoToken(token, resultadoUsuario.Dados.Codigo);

            return Resultado<DadosDoTokenDTO>.Sucesso(token);
        }

        public async Task<Resultado<DadosDoTokenDTO>> RefreshAcesso(DadosDoTokenDTO dadosDoToken)
        {
            var notificacoes = _tokenValidator.Validar(dadosDoToken);
            if (notificacoes.HasErrors())
                return Resultado<DadosDoTokenDTO>.Falha("Dados do token inválidos.");

            string codigoUsuario = dadosDoToken.Token != null
                ? await _tokenService.ObterCodigoUsuarioPorClaim(dadosDoToken.Token)
                : dadosDoToken.UsuarioCodigo;

            var refreshTokenExpirado = await _userIdentityService.RefreshTokenExpiradoServiceAsync(codigoUsuario);
            if (refreshTokenExpirado)
                return Resultado<DadosDoTokenDTO>.Falha("Token expirado ou inválido.");

            var usuario = await _usuarioService.ObterPorCodigoAsync(codigoUsuario);
            if (usuario?.Dados == null)
                return Resultado<DadosDoTokenDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dadosComplementares = await _dadosComplementaresDoUsuarioService
                .ObterInformacoesComplementaresDoUsuario(usuario.Dados);

            if (dadosComplementares == null)
                return Resultado<DadosDoTokenDTO>.Falha(ERRO_AUTENTICACAO);

            var dadosDeToken = await _tokenService.ObterTokenComRefreshToken(dadosComplementares);

            var autenticado = await _loginRepository.AutenticarUsuarioAsync(new UsuarioIdentity
            {
                Codigo = dadosComplementares.DadosDoUsuario.CodigoDoUsuario,
                Token = dadosDeToken.TokenDTO.Token,
                RefreshToken = dadosDeToken.RefrehTokenDTO.Token,
                RefreshTokenExpireTime = dadosDeToken.RefrehTokenDTO.Expires,
            });

            if (!autenticado)
                return Resultado<DadosDoTokenDTO>.Falha(ERRO_AUTENTICACAO);

            var token = new DadosDoTokenDTO(dadosDeToken.TokenDTO.Token, dadosDeToken.TokenDTO.Expires);

            _cacheUsuarioService.GravarCacheDeAcessoTokenInfo(dadosComplementares, dadosDeToken);
            _cookieService.CriarCookieDoToken(token, codigoUsuario);

            return Resultado<DadosDoTokenDTO>.Sucesso(token);
        }

        public async Task<Resultado> Logout(string usuarioCodigo)
        {
            var chaveDoCookie = $"{usuarioCodigo}{ETokenInfo.AcesssToken.GetDescription()}";
            _cookieService.RemoverCookie(chaveDoCookie);

            var refreshTokenRedefinido = await _userIdentityService.RedefinirRefreshTokenServiceAsync(usuarioCodigo);

            if (!refreshTokenRedefinido)
                return Resultado.Falha("Erro ao encerrar sessão.");

            await _loginRepository.Logout();
            return Resultado.Sucesso("Sessão encerrada com sucesso.");
        }        
    }
}