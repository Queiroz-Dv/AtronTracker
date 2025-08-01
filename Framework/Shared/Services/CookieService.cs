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