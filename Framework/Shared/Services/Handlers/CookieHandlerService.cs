using Microsoft.AspNetCore.Http;
using Shared.DTO.API;
using Shared.Enums;
using Shared.Extensions;
using Shared.Interfaces.Handlers;
using System.Security.Claims;

namespace Shared.Services.Handlers
{
    public class CookieHandlerService : ICookieHandlerService
    {
        private readonly IResponseCookies _cookies;
        private readonly ITokenHandlerService _tokenHandler;

        public CookieHandlerService(
            IResponseCookies responseCookies,
            ITokenHandlerService tokenHandler)
        {
            _cookies = responseCookies;
            _tokenHandler = tokenHandler;
        }

        public void CriarCookiesDoToken(InfoToken userInfo)
        {
            _cookies.Append(ETokenInfo.AcesssToken.GetDescription(), userInfo.Token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                Expires = userInfo.Expires
            });

            _cookies.Append(ETokenInfo.RefreshToken.GetDescription(), userInfo.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                Expires = userInfo.RefreshTokenExpireTime
            });
        }

        public InfoToken ExtrairInfoTokensDoCookie(HttpRequest request)
        {
            var accessToken = request.Cookies[ETokenInfo.AcesssToken.GetDescription()];
            var refreshToken = request.Cookies[ETokenInfo.RefreshToken.GetDescription()];

            if (accessToken != null) { return null; }

            var claimPrincipal = _tokenHandler.ObterClaimPrincipal(accessToken);

            if (claimPrincipal == null)
            {
                return null; // Ou lançar uma exceção, dependendo do seu fluxo de erro
            }

            var accessClaimExpires = claimPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Expiration)?.Value;
            var refreshClaimExpires = claimPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimCode.EXPIRACAO_REFRESH_TOKEN)?.Value;

            _ = DateTime.TryParse(accessClaimExpires, out DateTime accessTokenExpires);
            _ = DateTime.TryParse(refreshClaimExpires, out var refreshTokenExpires);

            return new InfoToken()
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Expires = accessTokenExpires,
                RefreshTokenExpireTime = refreshTokenExpires,
            };
        }
    }
}