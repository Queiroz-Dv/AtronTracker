using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;

namespace Shared.Application.Services
{
    public class CookieService : ICookieService
    {
        private readonly ICookieFactoryService _cookieFactory;

        public CookieService(ICookieFactoryService cookieFactory)
        {
            _cookieFactory = cookieFactory;
        }

        public void CriarCookieDoToken(DadosDoTokenDTO dadosDoToken, string codigoUsuario)
        {
            _cookieFactory.CriarCookieDoToken(dadosDoToken, codigoUsuario);
        }

        public async Task<DadosDoTokenDTO> ObterTokenRefreshTokenPorRequest(HttpRequest httpRequest)
        {
            return await _cookieFactory.ObterDadosDoTokenPorRequest(httpRequest);
        }

        public void RemoverCookie(string chave)
        {
            _cookieFactory.RemoverCookie(chave);
        }
    }
}