using Microsoft.AspNetCore.Http;
using Shared.DTO.API;
using Shared.Enums;
using Shared.Extensions;
using Shared.Interfaces.Factory;
using Shared.Interfaces.Services;
using System.Security.Claims;

namespace Shared.Services.Factory
{
    public class CookieFactory : CookieBuilder, ICookieFactory
    {        
        private readonly ITokenService _tokenService;

        public CookieFactory(IResponseCookies responseCookies, ITokenService tokenService) : base(responseCookies)
        {
            _tokenService = tokenService;
        }

        public void CriarCookiesDoToken(DadosDeTokenComRefreshToken tokenComRefreshToken)
        {
            MontarCookie(ETokenInfo.AcesssToken.GetDescription(), tokenComRefreshToken.TokenDTO.Token);

            MontarCookie(ETokenInfo.RefreshToken.GetDescription(),
                tokenComRefreshToken.RefrehTokenDTO.Token,
                tokenComRefreshToken.RefrehTokenDTO.Expires);           
        }

        public DadosDeTokenComRefreshToken ObterDadosDoTokenPorRequest(HttpRequest request)
        {
            var accessToken = request.Cookies[ETokenInfo.AcesssToken.GetDescription()];
            var refreshToken = request.Cookies[ETokenInfo.RefreshToken.GetDescription()];

            if (accessToken != null) { return null; }

            var claimPrincipal = _tokenService.ObterClaimPrincipal(accessToken);

            if (claimPrincipal == null)
            {
                return null; // Ou lançar uma exceção, dependendo do seu fluxo de erro
            }

            var accessClaimExpires = claimPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Expiration)?.Value;
            var refreshClaimExpires = claimPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimCode.EXPIRACAO_REFRESH_TOKEN)?.Value;

            _ = DateTime.TryParse(accessClaimExpires, out DateTime accessTokenExpires);
            _ = DateTime.TryParse(refreshClaimExpires, out var refreshTokenExpires);

            return new DadosDeTokenComRefreshToken()
            {
                TokenDTO = new DadosDoTokenDTO(accessToken, accessTokenExpires),
                RefrehTokenDTO = new DadosDoRefrehTokenDTO(refreshToken, refreshTokenExpires)
            };
        }
    }
}