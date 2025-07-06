using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.Interfaces.Contexts;
using Atron.Domain.Entities;
using Shared.DTO.API;
using Shared.DTO.API.Request;
using Shared.Extensions;
using Shared.Interfaces.Caching;
using Shared.Interfaces.Services;
using Shared.Interfaces.Validations;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices.ApplicationServices
{
    public class LoginUserService : ILoginUserService
    {
        private readonly ILoginContext _loginContext;
        private readonly ICacheService _cacheService;
        private readonly ICookieService _cookieService;
        private readonly IValidateModel<DadosDoTokenDTO> _validateToken;
        private readonly MessageModel _messageModel;

        const string ERRO_AUTENTICACAO = "Erro ao autenticar usuário. Verifique as informações e tente novamente.";

        public LoginUserService(
            ILoginContext loginContext,
            ICacheService cacheService,
            ICookieService cookieService,
            IValidateModel<DadosDoTokenDTO> validateToken,
            MessageModel messageModel)
        {
            _cookieService = cookieService;
            _cacheService = cacheService;
            _loginContext = loginContext;
            _validateToken = validateToken;
            _messageModel = messageModel;
        }

        public async Task<DadosDoTokenDTO> Autenticar(LoginRequestDTO loginRequest)
        {
            var dadosService = _loginContext.UsuarioContext.DadosComplementaresDoUsuarioService;
            var tokenService = _loginContext.AuthManagerContext.TokenService;
            var usuarioService = _loginContext.UsuarioContext.UsuarioService;
            var loginRepository = _loginContext.LoginRepository;
            var usuarioCacheService = _loginContext.UsuarioContext.CacheUsuarioService;

            var usuario = await usuarioService.ObterPorCodigoAsync(loginRequest.CodigoDoUsuario);

            if (usuario == null)
            {
                _messageModel.AddError("Usuário não encontrado.");
                return null;
            }

            var dadosComplementares = await dadosService.ObterInformacoesComplementaresDoUsuario(usuario);

            var dadosDoToken = await tokenService.ObterTokenComRefreshToken(dadosComplementares);

            var usuarioAutenticado = await loginRepository.AutenticarUsuarioAsync(new UsuarioIdentity()
            {
                Codigo = dadosComplementares.DadosDoUsuario.CodigoDoUsuario,
                Token = dadosDoToken.TokenDTO.Token,
                RefreshToken = dadosDoToken.RefrehTokenDTO.Token,
                RefreshTokenExpireTime = dadosDoToken.RefrehTokenDTO.Expires,
                Senha = loginRequest.Senha
            });

            if (!usuarioAutenticado)
            {
                _messageModel.AddError(ERRO_AUTENTICACAO);
                return null;
            }

            usuarioCacheService.GravarCacheDeAcessoTokenInfo(dadosComplementares, dadosDoToken);
            _cookieService.CriarCookiesDoToken(dadosDoToken);
            return new DadosDoTokenDTO(dadosDoToken.TokenDTO.Token, dadosDoToken.TokenDTO.Expires);
        }

        public async Task<DadosDoTokenDTO> RefreshAcesso(DadosDoTokenDTO dadosDoToken)
        {
            var dadosService = _loginContext.UsuarioContext.DadosComplementaresDoUsuarioService;
            var tokenService = _loginContext.AuthManagerContext.TokenService;
            var usuarioService = _loginContext.UsuarioContext.UsuarioService;
            var usuarioCacheService = _loginContext.UsuarioContext.CacheUsuarioService;
            var authRepository = _loginContext.AuthManagerContext.AppUserRepository;
            var loginRepository = _loginContext.LoginRepository;

            _validateToken.Validate(dadosDoToken);

            if (_messageModel.Messages.HasErrors()) return null;

            var codigoUsuario = await tokenService.ObterCodigoUsuarioPorClaim(dadosDoToken.Token);

            var refreshTokenDoUsuarioEstaExpirado = await authRepository.RefreshTokenExpirado(codigoUsuario);

            if (refreshTokenDoUsuarioEstaExpirado)
            {
                _messageModel.AddError("Token expirado ou inválido.");
                return null;
            }

            var usuario = await usuarioService.ObterPorCodigoAsync(codigoUsuario);

            if (usuario == null)
            {
                return null;
            }

            var dadosComplementares = await dadosService.ObterInformacoesComplementaresDoUsuario(usuario);

            if (dadosComplementares != null)
            {
                var dadosDeToken = await tokenService.ObterTokenComRefreshToken(dadosComplementares);

                var result = await loginRepository.AutenticarUsuarioAsync(new UsuarioIdentity()
                {
                    Codigo = dadosComplementares.DadosDoUsuario.CodigoDoUsuario,
                    Token = dadosDeToken.TokenDTO.Token,
                    RefreshToken = dadosDeToken.RefrehTokenDTO.Token,
                    RefreshTokenExpireTime = dadosDeToken.RefrehTokenDTO.Expires,
                });

                if (!result)
                {
                    _messageModel.AddError(ERRO_AUTENTICACAO);
                    return null;
                }

                usuarioCacheService.GravarCacheDeAcessoTokenInfo(dadosComplementares, dadosDeToken);
                return new DadosDoTokenDTO(dadosDeToken.TokenDTO.Token, dadosDeToken.TokenDTO.Expires);
            }

            return null;
        }

        public async Task Logout(string usuarioCodigo)
        {  
            var  appUserRepository = _loginContext.AuthManagerContext.AppUserRepository;

            _cacheService.RemoverCache(ECacheKeysInfo.Acesso, usuarioCodigo);
            _cacheService.RemoverCache(ECacheKeysInfo.TokenInfo, usuarioCodigo);
            await appUserRepository.RedefinirRefreshToken(usuarioCodigo);
            await _loginContext.LoginRepository.Logout();
        }

        public async Task<bool> TrocarSenha(LoginRequestDTO dto)
        {
            return await _loginContext.LoginRepository.AtualizarSenhaUsuario(dto.CodigoDoUsuario, dto.Senha);
        }
    }
}