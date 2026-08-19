using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;

namespace Shared.Application.Services
{
    public class CookieService(ICookieFactoryService cookieFactory) : ICookieService
    {
        private readonly ICookieFactoryService _cookieFactory = cookieFactory;

        public async Task<DadosDoRefreshTokenCookieDTO> ObterRefreshTokenPorRequest(HttpRequest httpRequest)
        {
            return await _cookieFactory.ObterRefreshTokenPorRequest(httpRequest);
        }

        public void CriarCookieDeRefreshToken(DadosDoRefrehTokenDTO dadosDoRefreshToken)
        {
            _cookieFactory.CriarCookieDeRefreshToken(dadosDoRefreshToken);
        }

        public void RemoverCookieDeRefreshToken()
        {
            _cookieFactory.RemoverCookieDeRefreshToken();
        }
    }
}