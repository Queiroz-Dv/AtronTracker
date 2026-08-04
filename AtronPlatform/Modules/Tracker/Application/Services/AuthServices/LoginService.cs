using Application.Interfaces.ApplicationInterfaces;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Application.DTO.Request;
using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System;
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

        public LoginService(
            ILoginRepository loginRepository,
            IUsuarioService usuarioService,
            IDadosComplementaresDoUsuarioService dadosComplementaresDoUsuarioService,
            ITokenService tokenService,
            ICacheUsuarioService cacheUsuarioService,
            ICookieService cookieService,
            IUserIdentityService userIdentityService)
        {
            _loginRepository = loginRepository;
            _usuarioService = usuarioService;
            _dadosComplementaresDoUsuarioService = dadosComplementaresDoUsuarioService;
            _tokenService = tokenService;
            _cacheUsuarioService = cacheUsuarioService;
            _cookieService = cookieService;
            _userIdentityService = userIdentityService;
        }

        public async Task<Resultado<DadosDoTokenDTO>> Autenticar(LoginRequestDTO loginRequest)
        {
            var resultadoUsuario = await _usuarioService.ObterPorCodigoAsync(loginRequest.CodigoDoUsuario);

            if (resultadoUsuario?.Dados == null)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);

            var credenciaisValidas = await _loginRepository.ValidarCredenciaisAsync(
                resultadoUsuario.Dados.Codigo,
                loginRequest.Senha);

            if (!credenciaisValidas)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);

            if (!resultadoUsuario.Dados.EmailConfirmado)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);

            var dadosComplementares = await _dadosComplementaresDoUsuarioService
                .ObterInformacoesComplementaresDoUsuario(resultadoUsuario.Dados);

            var dadosDoToken = await _tokenService.ObterTokenComRefreshToken(dadosComplementares);

            var usuarioAutenticado = await _userIdentityService.GravarRefreshTokenAsync(
                dadosComplementares.DadosDoUsuario.CodigoDoUsuario,
                dadosDoToken.RefrehTokenDTO.Token,
                dadosDoToken.RefrehTokenDTO.Expires);

            if (!usuarioAutenticado)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);

            var token = new DadosDoTokenDTO(dadosDoToken.TokenDTO.Token, dadosDoToken.TokenDTO.Expires)
            {
                UsuarioCodigo = dadosComplementares.DadosDoUsuario.CodigoDoUsuario
            };

            _cacheUsuarioService.GravarCacheDeAcesso(dadosComplementares, dadosDoToken.TokenDTO.Expires);
            _cookieService.CriarCookieDeRefreshToken(dadosDoToken.RefrehTokenDTO);

            return Resultado<DadosDoTokenDTO>.Sucesso(token);
        }

        public async Task<Resultado<DadosDoTokenDTO>> RefreshAcesso(DadosDoRefreshTokenCookieDTO dadosDoRefreshToken)
        {
            if (dadosDoRefreshToken is null || !dadosDoRefreshToken.IsValid())
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_DadosRefreshTokenInvalido);

            var sessaoRefreshToken = await _userIdentityService.ObterSessaoRefreshTokenAsync(dadosDoRefreshToken.RefreshToken);
            if (sessaoRefreshToken is null)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_DadosRefreshTokenInvalido);

            if (sessaoRefreshToken.ExpiraEm <= DateTime.UtcNow)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_TokenExpiradoInvalido);

            var codigoUsuario = sessaoRefreshToken.UsuarioCodigo;
            var usuario = await _usuarioService.ObterPorCodigoAsync(codigoUsuario);
            if (usuario?.Dados == null)
                return Resultado<DadosDoTokenDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dadosComplementares = await _dadosComplementaresDoUsuarioService
                .ObterInformacoesComplementaresDoUsuario(usuario.Dados);

            if (dadosComplementares == null)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);

            var dadosDeToken = await _tokenService.ObterTokenComRefreshToken(dadosComplementares);

            var autenticado = await _userIdentityService.RotacionarRefreshTokenAsync(new RotacaoRefreshToken(
                codigoUsuario,
                dadosDoRefreshToken.RefreshToken,
                dadosDeToken.RefrehTokenDTO.Token,
                dadosDeToken.RefrehTokenDTO.Expires));

            if (!autenticado)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);

            var token = new DadosDoTokenDTO(dadosDeToken.TokenDTO.Token, dadosDeToken.TokenDTO.Expires)
            {
                UsuarioCodigo = codigoUsuario
            };

            _cacheUsuarioService.GravarCacheDeAcesso(dadosComplementares, dadosDeToken.TokenDTO.Expires);
            _cookieService.CriarCookieDeRefreshToken(dadosDeToken.RefrehTokenDTO);

            return Resultado<DadosDoTokenDTO>.Sucesso(token);
        }

        public async Task<Resultado> Logout(string usuarioCodigo)
        {
            _cookieService.RemoverCookieDeRefreshToken();

            var refreshTokenRedefinido = await _userIdentityService.RevogarRefreshTokenAsync(usuarioCodigo);

            if (!refreshTokenRedefinido)
                return Resultado.Falha(AuthResource.Erro_EncerrarSessao);

            _cacheUsuarioService.RemoverCacheDeAcessoTokenInfo(usuarioCodigo);
            return Resultado.Sucesso(AuthResource.Mensagem_SessaoEncerrada);
        }
    }
}
