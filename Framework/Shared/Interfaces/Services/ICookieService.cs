using Microsoft.AspNetCore.Http;
using Shared.DTO.API;

namespace Shared.Interfaces.Services
{
    public interface ICookieService
    {
        DadosDeTokenComRefreshToken ObterTokenRefreshTokenPorRequest(HttpRequest httpRequest);

        void CriarCookiesDoToken(DadosDeTokenComRefreshToken tokenComRefreshToken);
    }
}