using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.ApiServices.AuthServices.Bases;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Shared.DTO.API;
using Shared.DTO.API.Request;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Interfaces.Caching;
using Shared.Interfaces.Validations;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices.AuthServices
{
    public class LoginService : LoginBaseService, ILoginService
    {
        //private readonly IValidateModel<DadosDoTokenDTO> _validateToken;
        //private readonly MessageModel _messageModel;

        const string ERRO_AUTENTICACAO = "Erro ao autenticar usuário. Verifique as informações e tente novamente.";

        public LoginService(
            IServiceAccessor serviceAccessor,
            ILoginRepository loginRepository) : base(serviceAccessor, loginRepository)
        {
            //_validateToken = validateToken;
            //_messageModel = messageModel;
        }

        public async Task<DadosDoTokenDTO> Autenticar(LoginRequestDTO loginRequest)
        {
            var usuario = await UsuarioService.ObterPorCodigoAsync(loginRequest.CodigoDoUsuario);

            if (usuario == null)
            {
                Messages.AddError("Usuário não encontrado.");
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
                Messages.AddError(ERRO_AUTENTICACAO);
                return null;
            }

            CacheUsuarioService.GravarCacheDeAcessoTokenInfo(dadosComplementares, dadosDoToken);
            CookieService.CriarCookiesDoToken(dadosDoToken);
            return new DadosDoTokenDTO(dadosDoToken.TokenDTO.Token, dadosDoToken.TokenDTO.Expires);
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
                Messages.AddError("Token expirado ou inválido.");
                return null;
            }

            var usuario = await UsuarioService.ObterPorCodigoAsync(codigoUsuario);

            if (usuario == null) return null;

            var dadosComplementares = await DadosComplementaresDoUsuarioService.ObterInformacoesComplementaresDoUsuario(usuario);

            if (dadosComplementares != null)
            {
                var dadosDeToken = await TokenService.ObterTokenComRefreshToken(dadosComplementares);

                var result = await _loginRepository.AutenticarUsuarioAsync(new UsuarioIdentity()
                {
                    Codigo = dadosComplementares.DadosDoUsuario.CodigoDoUsuario,
                    Token = dadosDeToken.TokenDTO.Token,
                    RefreshToken = dadosDeToken.RefrehTokenDTO.Token,
                    RefreshTokenExpireTime = dadosDeToken.RefrehTokenDTO.Expires,
                });

                if (!result)
                {
                    Messages.AddError(ERRO_AUTENTICACAO);
                    return null;
                }

                CacheUsuarioService.GravarCacheDeAcessoTokenInfo(dadosComplementares, dadosDeToken);
                CookieService.CriarCookiesDoToken(dadosDeToken);
                return new DadosDoTokenDTO(dadosDeToken.TokenDTO.Token, dadosDeToken.TokenDTO.Expires);
            }

            return null;
        }

        public async Task Logout(string usuarioCodigo)
        {
            var cacheService = _serviceAccessor.ObterService<ICacheService>();

            cacheService.RemoverCache(ECacheKeysInfo.Acesso, usuarioCodigo);
            cacheService.RemoverCache(ECacheKeysInfo.TokenInfo, usuarioCodigo);
            await UserIdentityService.RedefinirRefreshTokenServiceAsync(usuarioCodigo);
            await _loginRepository.Logout();
        }

        public async Task<bool> TrocarSenha(LoginRequestDTO dto)
        {
            return await _loginRepository.AtualizarSenhaUsuario(dto.CodigoDoUsuario, dto.Senha);
        }
    }
}