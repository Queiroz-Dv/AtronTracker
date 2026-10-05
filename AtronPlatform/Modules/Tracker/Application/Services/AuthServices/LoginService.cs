using Application.Interfaces.ApplicationInterfaces;
using Application.Interfaces.Services;
using Application.UseCases.UsuarioCases;
using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Application.Security;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.Services.AuthServices
{
    public class LoginService(
        ILoginRepository _loginRepository,
        ObterUsuarioCase _usuarioService,
        IDadosComplementaresDoUsuarioService _dadosComplementaresDoUsuarioService,
        ITokenService _tokenService,
        ICacheUsuarioService _cacheUsuarioService,
        ICookieService _cookieService,
        IUsuarioIdentityRepository _userIdentityRepository) : ILoginService
    {
        public async Task<Resultado<DadosDoTokenDTO>> Autenticar(LoginRequestDTO loginRequest)
        {
            var resultadoUsuario = await _usuarioService.ExecutarAsync(loginRequest.CodigoDoUsuario);

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

            if (dadosComplementares.DadosDoUsuario.Workspace.IsNullable())
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_AcessoBloqueadoUsuarioSemWorkspace);

            if (!dadosComplementares.DadosDoUsuario.EhResponsavelDoWorkspace
                && (dadosComplementares.DadosDoUsuario.CodigoDoCargo.IsNullOrEmpty()
                    || dadosComplementares.DadosDoUsuario.CodigoDoDepartamento.IsNullOrEmpty()))
            {
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_AcessoBloqueadoMembroSemVinculo);
            }

            var dadosDoToken = await _tokenService.ObterTokenComRefreshToken(dadosComplementares);

            if (dadosComplementares.DadosDoUsuario.CodigoDoUsuario.IsNullOrEmpty() ||
                dadosDoToken.RefrehTokenDTO.Value.IsNullOrEmpty() ||
                dadosDoToken.RefrehTokenDTO.Expires <= DateTime.UtcNow)
            {
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);
            }

            var usuarioAutenticado = await _userIdentityRepository.AtualizarRefreshTokenUsuarioRepositoryAsync(
                dadosComplementares.DadosDoUsuario.CodigoDoUsuario,
                RefreshTokenHash.Obter(dadosDoToken.RefrehTokenDTO.Value),
                dadosDoToken.RefrehTokenDTO.Expires);

            if (!usuarioAutenticado)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);

            _cacheUsuarioService.GravarCacheDeAcesso(dadosComplementares, dadosDoToken.TokenDTO.Expires);
            _cookieService.CriarCookieDeRefreshToken(dadosDoToken.RefrehTokenDTO);

            return Resultado<DadosDoTokenDTO>.Sucesso(dadosDoToken.TokenDTO);
        }

        public async Task<Resultado<DadosDoTokenDTO>> RefreshAcesso(DadosDoRefreshTokenCookieDTO dadosDoRefreshToken)
        {
            if (dadosDoRefreshToken is null || !dadosDoRefreshToken.IsValid() || dadosDoRefreshToken.RefreshToken.IsNullOrEmpty())
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_DadosRefreshTokenInvalido);

            var sessaoRefreshToken = await _userIdentityRepository.ObterSessaoRefreshTokenRepositoryAsync(
                RefreshTokenHash.Obter(dadosDoRefreshToken.RefreshToken));

            if (sessaoRefreshToken is null)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_DadosRefreshTokenInvalido);

            if (sessaoRefreshToken.ExpiraEm <= DateTime.UtcNow)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_TokenExpiradoInvalido);

            var codigoUsuario = sessaoRefreshToken.UsuarioCodigo;
            var usuario = await _usuarioService.ExecutarAsync(codigoUsuario);
            if (usuario?.Dados == null)
                return Resultado<DadosDoTokenDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dadosComplementares = await _dadosComplementaresDoUsuarioService.ObterInformacoesComplementaresDoUsuario(usuario.Dados);

            if (dadosComplementares.IsNullable() || dadosComplementares.DadosDoUsuario.Workspace.IsNullable())
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);

            if (!dadosComplementares.DadosDoUsuario.EhResponsavelDoWorkspace
                && (dadosComplementares.DadosDoUsuario.CodigoDoCargo.IsNullOrEmpty()
                    || dadosComplementares.DadosDoUsuario.CodigoDoDepartamento.IsNullOrEmpty()))
            {
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);
            }

            var dadosDeToken = await _tokenService.ObterTokenComRefreshToken(dadosComplementares);

            if (codigoUsuario.IsNullOrEmpty() ||
                dadosDoRefreshToken.RefreshToken.IsNullOrEmpty() ||
                dadosDeToken.RefrehTokenDTO.Value.IsNullOrEmpty() ||
                dadosDeToken.RefrehTokenDTO.Expires <= DateTime.UtcNow)
            {
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);
            }

            var autenticado = await _userIdentityRepository.RotacionarRefreshTokenRepositoryAsync(new RotacaoRefreshTokenHash(
                codigoUsuario,
                RefreshTokenHash.Obter(dadosDoRefreshToken.RefreshToken),
                RefreshTokenHash.Obter(dadosDeToken.RefrehTokenDTO.Value),
                dadosDeToken.RefrehTokenDTO.Expires));

            if (!autenticado)
                return Resultado<DadosDoTokenDTO>.Falha(AuthResource.Erro_Autenticacao);

            var token = new DadosDoTokenDTO(dadosDeToken.TokenDTO.Value, dadosDeToken.TokenDTO.Expires) { UsuarioCodigo = codigoUsuario };

            _cacheUsuarioService.GravarCacheDeAcesso(dadosComplementares, dadosDeToken.TokenDTO.Expires);
            _cookieService.CriarCookieDeRefreshToken(dadosDeToken.RefrehTokenDTO);

            return Resultado<DadosDoTokenDTO>.Sucesso(token);
        }

        public async Task<Resultado> Logout(string usuarioCodigo)
        {
            _cookieService.RemoverCookieDeRefreshToken();

            if (usuarioCodigo.IsNullOrEmpty())
                return Resultado.Falha(AuthResource.Erro_EncerrarSessao);

            var refreshTokenRedefinido = await _userIdentityRepository.RedefinirRefreshTokenRepositoryAsync(usuarioCodigo);

            if (!refreshTokenRedefinido)
                return Resultado.Falha(AuthResource.Erro_EncerrarSessao);

            _cacheUsuarioService.RemoverCacheDeAcessoTokenInfo(usuarioCodigo);
            return Resultado.Sucesso(AuthResource.Mensagem_SessaoEncerrada);
        }
    }
}