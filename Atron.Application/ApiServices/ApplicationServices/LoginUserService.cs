using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Application.Interfaces;
using Atron.Application.Interfaces.Handlers;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Shared.DTO.API;
using Shared.DTO.API.Request;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Interfaces.Caching;
using Shared.Interfaces.Handlers;
using Shared.Interfaces.Validations;
using Shared.Models;
using System;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices.ApplicationServices
{
    public class LoginUserService : ILoginUserService
    {
        private readonly IApplicationTokenService _tokenService;
        private readonly ITokenHandlerService _tokenHandler;
        private readonly IUsuarioHandler _usuarioHandler;
        private readonly ILoginApplicationRepository _loginApplication;
        private readonly IUsuarioService _usuarioService;
        private readonly IValidateModel<InfoToken> _valideInfoToken;
        private readonly ICacheService _cacheService;
        private readonly MessageModel _messageModel;

        const string ERRO_AUTENTICACAO = "Erro ao autenticar usuário. Verifique as informações e tente novamente.";

        public LoginUserService(
            ILoginApplicationRepository loginApplication,
            IUsuarioService usuarioService,
            IUsuarioHandler usuarioHandler,
            IApplicationTokenService tokenService,
            ITokenHandlerService tokenHandler,
            IValidateModel<InfoToken> valideInfoToken,
            MessageModel messageModel,
            ICacheService cacheService)
        {
            _loginApplication = loginApplication;
            _tokenService = tokenService;
            _usuarioService = usuarioService;
            _messageModel = messageModel;
            _valideInfoToken = valideInfoToken;
            _tokenHandler = tokenHandler;
            _usuarioHandler = usuarioHandler;
            _cacheService = cacheService;
        }

        public async Task<LoginDTO> Authenticate(LoginRequestDTO loginRequest)
        {
            var usuario = await _usuarioService.ObterPorCodigoAsync(loginRequest.CodigoDoUsuario);

            var loginDTO = await _usuarioHandler.PreencherInformacoesDeUsuarioParaLoginAsync(usuario);

            loginDTO.UserToken = await _tokenService.GerarToken(loginDTO.DadosDoUsuario);

            var result = await _loginApplication.AutenticarUsuarioAsync(new UsuarioIdentity()
            {
                Codigo = loginDTO.DadosDoUsuario.CodigoDoUsuario,
                Token = loginDTO.UserToken.Token,
                RefreshToken = loginDTO.UserToken.RefreshToken,
                RefreshTokenExpireTime = loginDTO.UserToken.RefreshTokenExpireTime,
                Senha = loginRequest.Senha
            });

            if (!result)
            {
                _messageModel.AddError(ERRO_AUTENTICACAO);
                return null;
            }

            GravarCacheDeAcessoTokenInfo(loginRequest.CodigoDoUsuario, loginDTO.DadosDoUsuario, loginDTO.UserToken);

            return loginDTO;
        }

        private void GravarCacheDeAcessoTokenInfo(string codigoUsuario, DadosDoUsuario dadosDoUsuario, InfoToken infoToken)
        {
            var acessoCacheInfo = new CacheInfo<DadosDoUsuario>(ECacheKeysInfo.Acesso,codigoUsuario)
            {
                EntityInfo = dadosDoUsuario,
                ExpireTime = infoToken.Expires
            };

            var tokenUsuarioInfo = new CacheInfo<InfoToken>(ECacheKeysInfo.TokenInfo,codigoUsuario)
            {
                EntityInfo = infoToken,
                ExpireTime = infoToken.Expires
            };

            _cacheService.GravarCache(acessoCacheInfo);
            _cacheService.GravarCache(tokenUsuarioInfo);
        }

        public async Task<InfoToken> RefreshAcesso(InfoToken infoToken)
        {
            _valideInfoToken.Validate(infoToken);

            if (_messageModel.Messages.HasErrors()) return null;

            var codigoUsuario = _tokenHandler.ObterCodigoUsuarioPorClaim(infoToken.Token);

            var refreshTokenDoUsuarioEstaExpirado = await _usuarioService.TokenDeUsuarioExpiradoServiceAsync(codigoUsuario, infoToken.RefreshToken);

            if (refreshTokenDoUsuarioEstaExpirado)
            {
                _messageModel.AddError("Token expirado ou inválido.");
                return null;
            }

            var usuario = await _usuarioService.ObterPorCodigoAsync(codigoUsuario);

            var loginDTO = await _usuarioHandler.PreencherInformacoesDeUsuarioParaLoginAsync(usuario);

            if (loginDTO != null)
            {
                var novoInfoToken = await _tokenService.GerarToken(loginDTO.DadosDoUsuario);

                var result = await _loginApplication.AutenticarUsuarioAsync(new UsuarioIdentity()
                {
                    Codigo = loginDTO.DadosDoUsuario.CodigoDoUsuario,
                    Token = novoInfoToken.Token,
                    RefreshToken = novoInfoToken.RefreshToken,
                    RefreshTokenExpireTime = novoInfoToken.RefreshTokenExpireTime
                });

                if (!result)
                {
                    _messageModel.AddError(ERRO_AUTENTICACAO);
                    return null;
                }

                GravarCacheDeAcessoTokenInfo(codigoUsuario, loginDTO.DadosDoUsuario, novoInfoToken);
                return novoInfoToken;
            }

            return null;
        }

        public async Task Logout()
        {
            await _loginApplication.Logout();
        }

        public async Task<bool> TrocarSenha(LoginRequestDTO dto)
        {
            var login = new ApiLogin { UserName = dto.CodigoDoUsuario, Password = dto.Senha };

            return await _loginApplication.ConfigPasswordAsync(dto.CodigoDoUsuario, login);
        }
    }
}