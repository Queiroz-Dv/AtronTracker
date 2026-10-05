using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;

namespace Shared.Application.Services
{
    public class CookieService : ICookieService
    {
        public const string NomeCookieRefreshToken = "ATRON_REFRESH_TOKEN";
        private readonly IResponseCookies _cookies;

        public CookieService(IResponseCookies cookies)
        {
            _cookies = cookies;
        }

        public void CriarCookieDeRefreshToken(DadosDoRefrehTokenDTO dadosDoRefreshToken)
        {
            _cookies.Append(NomeCookieRefreshToken, dadosDoRefreshToken.Value, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Path = "/",
                Expires = dadosDoRefreshToken.Expires
            });
        }

        public Task<DadosDoRefreshTokenCookieDTO> ObterRefreshTokenPorRequest(HttpRequest request)
        {
            if (!request.Cookies.TryGetValue(NomeCookieRefreshToken, out var refreshToken))
                return Task.FromResult<DadosDoRefreshTokenCookieDTO>(null);

            return Task.FromResult(new DadosDoRefreshTokenCookieDTO
            {
                RefreshToken = refreshToken
            });
        }

        public void RemoverCookieDeRefreshToken()
        {
            _cookies.Delete(NomeCookieRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Path = "/"
            });
        }
    }
}