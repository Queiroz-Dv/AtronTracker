using Microsoft.AspNetCore.Http;
using Shared.DTO.API;
using Shared.Interfaces.Factory;
using Shared.Interfaces.Services;

namespace Shared.Services
{
    public class CookieService : ICookieService
    {
        private readonly ICookieFactory _cookieFactory;

        public CookieService(ICookieFactory cookieFactory)
        {
            _cookieFactory = cookieFactory;
        }

        public void CriarCookiesDoToken(DadosDeTokenComRefreshToken tokenComRefreshToken)
        {
            _cookieFactory.CriarCookiesDoToken(tokenComRefreshToken);
        }

        public DadosDeTokenComRefreshToken ObterTokenRefreshTokenPorRequest(HttpRequest httpRequest)
        {
            return _cookieFactory.ObterDadosDoTokenPorRequest(httpRequest);
        }
    }
}