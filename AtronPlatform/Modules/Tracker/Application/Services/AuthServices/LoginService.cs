using Application.Interfaces.ApplicationInterfaces;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Application.UseCases.UsuarioCases;
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
        private readonly ReenviarConfirmacaoEmail _reenviarConfirmacaoEmail;

        public LoginService(
            ILoginRepository loginRepository,
            IUsuarioService usuarioService,
            IDadosComplementaresDoUsuarioService dadosComplementaresDoUsuarioService,
            ITokenService tokenService,
            ICacheUsuarioService cacheUsuarioService,
            ICookieService cookieService,
            IUserIdentityService userIdentityService,
            ReenviarConfirmacaoEmail reenviarConfirmacaoEmail)
        {
            _loginRepository = loginRepository;
            _usuarioService = usuarioService;
            _dadosComplementaresDoUsuarioService = dadosComplementaresDoUsuarioService;
            _tokenService = tokenService;
            _cacheUsuarioService = cacheUsuarioService;
            _cookieService = cookieService;
            _userIdentityService = userIdentityService;
            _reenviarConfirmacaoEmail = reenviarConfirmacaoEmail;
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
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);

            if (!resultadoUsuario.Dados.EmailConfirmado)
            {
                var resultadoReenvio = await _reenviarConfirmacaoEmail.ExecutarAsync(
                    resultadoUsuario.Dados.Codigo,
                    loginRequest.ClientUri);

                if (resultadoReenvio.TeveFalha)
                    return Resultado<DadosDoTokenDTO>.Falhas(resultadoReenvio.Messages);

                return Resultado<DadosDoTokenDTO>.Falha(EmailResource.Erro_EmailNaoConfirmado);
            }

            var token = new DadosDoTokenDTO(dadosDoToken.TokenDTO.Token, dadosDoToken.TokenDTO.Expires);

            _cacheUsuarioService.GravarCacheDeAcessoTokenInfo(dadosComplementares, dadosDoToken);
            _cookieService.CriarCookieDeRefreshToken(dadosDoToken.RefrehTokenDTO, resultadoUsuario.Dados.Codigo);

            return Resultado<DadosDoTokenDTO>.Sucesso(token);
        }

        public async Task<Resultado<DadosDoTokenDTO>> RefreshAcesso(DadosDoRefreshTokenCookieDTO dadosDoRefreshToken)
        {
            if (dadosDoRefreshToken is null || !dadosDoRefreshToken.IsValid())
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_DadosRefreshTokenInvalido);

            var codigoUsuario = dadosDoRefreshToken.UsuarioCodigo;

            var refreshTokenAtual = await _userIdentityService.ObterRefreshTokenPorCodigoUsuarioServiceAsync(codigoUsuario);
            if (refreshTokenAtual.IsNullOrEmpty() || refreshTokenAtual != dadosDoRefreshToken.RefreshToken)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_DadosRefreshTokenInvalido);

            var refreshTokenExpirado = await _userIdentityService.RefreshTokenExpiradoServiceAsync(codigoUsuario);
            if (refreshTokenExpirado)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_TokenExpiradoInvalido);

            var usuario = await _usuarioService.ObterPorCodigoAsync(codigoUsuario);
            if (usuario?.Dados == null)
                return Resultado<DadosDoTokenDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dadosComplementares = await _dadosComplementaresDoUsuarioService
                .ObterInformacoesComplementaresDoUsuario(usuario.Dados);

            if (dadosComplementares == null)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);

            var dadosDeToken = await _tokenService.ObterTokenComRefreshToken(dadosComplementares);

            var autenticado = await _loginRepository.AutenticarUsuarioAsync(new UsuarioIdentity
            {
                Codigo = dadosComplementares.DadosDoUsuario.CodigoDoUsuario,
                Token = dadosDeToken.TokenDTO.Token,
                RefreshToken = dadosDeToken.RefrehTokenDTO.Token,
                RefreshTokenExpireTime = dadosDeToken.RefrehTokenDTO.Expires,
            });

            if (!autenticado)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);

            var token = new DadosDoTokenDTO(dadosDeToken.TokenDTO.Token, dadosDeToken.TokenDTO.Expires);

            _cacheUsuarioService.GravarCacheDeAcessoTokenInfo(dadosComplementares, dadosDeToken);
            _cookieService.CriarCookieDeRefreshToken(dadosDeToken.RefrehTokenDTO, codigoUsuario);

            return Resultado<DadosDoTokenDTO>.Sucesso(token);
        }

        public async Task<Resultado> Logout(string usuarioCodigo)
        {
            var chaveDoCookie = $"{usuarioCodigo}{ETokenInfo.RefreshToken.GetDescription()}";
            _cookieService.RemoverCookie(chaveDoCookie);

            var refreshTokenRedefinido = await _userIdentityService.RedefinirRefreshTokenServiceAsync(usuarioCodigo);

            if (!refreshTokenRedefinido)
                return Resultado.Falha(AuthResource.Erro_EncerrarSessao);

            await _loginRepository.Logout();
            return Resultado.Sucesso(AuthResource.Mensagem_SessaoEncerrada);
        }
    }
}
