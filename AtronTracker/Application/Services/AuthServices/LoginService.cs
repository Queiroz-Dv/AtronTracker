using Application.Interfaces.ApplicationInterfaces;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Application.Services.AuthServices.Bases;
using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace Application.Services.AuthServices
{
    public class LoginService : ServiceBase, ILoginService
    {
        const string ERRO_AUTENTICACAO = "Erro ao autenticar usuário. Verifique as informações e tente novamente.";
        private readonly ILoginRepository _loginRepository;

        private DadosDoTokenDTO CriarToken(string token, DateTime expires) => new(token, expires);

        public LoginService(IAccessorService serviceAccessor, ILoginRepository loginRepository) : base(serviceAccessor)
        {
            _loginRepository = loginRepository;
        }

        public async Task<DadosDoTokenDTO> Autenticar(LoginRequestDTO loginRequest)
        {
            var usuario = await UsuarioService.ObterPorCodigoAsync(loginRequest.CodigoDoUsuario);
            if (usuario == null)
            {
                Messages.AdicionarErro("Usuário não encontrado.");
                return null;
            }

            var dadosComplementares = await DadosComplementaresDoUsuarioService.ObterInformacoesComplementaresDoUsuario(usuario);

            var dadosDoToken = await TokenService.ObterTokenComRefreshToken(dadosComplementares);

            var usuarioAutenticado = await _loginRepository.AutenticarUsuarioAsync(new UsuarioIdentity()
            {
                Codigo = dadosComplementares.DadosDoUsuario.CodigoDoUsuario,
                Token = dadosDoToken.TokenDTO.Token,
                RefreshToken = dadosDoToken.RefrehTokenDTO.Token,
                RefreshTokenExpireTime = dadosDoToken.RefrehTokenDTO.Expires,
                Senha = loginRequest.Senha
            });

            if (!usuarioAutenticado)
            {
                Messages.AdicionarErro(ERRO_AUTENTICACAO);
                return null;
            }

            var token = CriarToken(dadosDoToken.TokenDTO.Token, dadosDoToken.TokenDTO.Expires);

            CacheUsuarioService.GravarCacheDeAcessoTokenInfo(dadosComplementares, dadosDoToken);
            CookieService.CriarCookieDoToken(token, usuario.Codigo);
            return token;
        }

        public async Task<DadosDoTokenDTO> RefreshAcesso(DadosDoTokenDTO dadosDoToken)
        {
            var validarDTO = GetValidator<DadosDoTokenDTO>();

            validarDTO.Validate(dadosDoToken);

            if (Messages.Notificacoes.HasErrors()) return null;

            var codigoUsuario = await TokenService.ObterCodigoUsuarioPorClaim(dadosDoToken.Token);

            var refreshTokenDoUsuarioEstaExpirado = await UserIdentityService.RefreshTokenExpiradoServiceAsync(codigoUsuario);

            if (refreshTokenDoUsuarioEstaExpirado)
            {
                Messages.AdicionarErro("Token expirado ou inválido.");
                return null;
            }

            var usuario = await UsuarioService.ObterPorCodigoAsync(codigoUsuario);

            if (usuario == null) return null;

            var dadosComplementares = await DadosComplementaresDoUsuarioService.ObterInformacoesComplementaresDoUsuario(usuario);

            if (dadosComplementares != null)
            {
                var dadosDeToken = await TokenService.ObterTokenComRefreshToken(dadosComplementares);

                var autenticado = await _loginRepository.AutenticarUsuarioAsync(new UsuarioIdentity()
                {
                    Codigo = dadosComplementares.DadosDoUsuario.CodigoDoUsuario,
                    Token = dadosDeToken.TokenDTO.Token,
                    RefreshToken = dadosDeToken.RefrehTokenDTO.Token,
                    RefreshTokenExpireTime = dadosDeToken.RefrehTokenDTO.Expires,
                });

                if (!autenticado)
                {
                    Messages.AdicionarErro(ERRO_AUTENTICACAO);
                    return null;
                }

                var token = CriarToken(dadosDeToken.TokenDTO.Token, dadosDeToken.TokenDTO.Expires);

                CacheUsuarioService.GravarCacheDeAcessoTokenInfo(dadosComplementares, dadosDeToken);
                CookieService.CriarCookieDoToken(token, codigoUsuario);
                return token;
            }

            return null;
        }

        public async Task<bool> Logout(string usuarioCodigo)
        {
            var cacheService = ObterService<ICacheService>();
            var identityService = ObterService<IUserIdentityService>();
            var cookieService = ObterService<ICookieService>();

            cacheService.RemoverCache(ECacheKeysInfo.Acesso, usuarioCodigo);
            cacheService.RemoverCache(ECacheKeysInfo.TokenInfo, usuarioCodigo);

            var chaveDoCookie = $"{usuarioCodigo}{ETokenInfo.AcesssToken.GetDescription()}";

            cookieService.RemoverCookie(chaveDoCookie);

            var refreshTokenRedefinido = await identityService.RedefinirRefreshTokenServiceAsync(usuarioCodigo);

            if (refreshTokenRedefinido)
            {
                await _loginRepository.Logout();
                return refreshTokenRedefinido;
            }

            return false;
        }

        public async Task<bool> TrocarSenha(LoginRequestDTO dto)
        {
            return await _loginRepository.AtualizarSenhaUsuario(dto.CodigoDoUsuario, dto.Senha);
        }

        #region Services
        private IUsuarioService UsuarioService => ObterService<IUsuarioService>();
        private IDadosComplementaresDoUsuarioService DadosComplementaresDoUsuarioService => ObterService<IDadosComplementaresDoUsuarioService>();
        private ITokenService TokenService => ObterService<ITokenService>();
        private ICacheUsuarioService CacheUsuarioService => ObterService<ICacheUsuarioService>();
        private ICookieService CookieService => ObterService<ICookieService>();
        private IUserIdentityService UserIdentityService => ObterService<IUserIdentityService>();
        #endregion
    }
}