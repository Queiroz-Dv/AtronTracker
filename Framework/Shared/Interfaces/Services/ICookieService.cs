using Microsoft.AspNetCore.Http;
using Shared.DTO.API;

namespace Shared.Interfaces.Services
{
    public interface ICookieService
    {
        DadosDeTokenComRefreshToken ObterTokenRefreshTokenPorRequest(HttpRequest httpRequest);

        void CriarCookieDoToken(DadosDoTokenDTO dadosDoToken, string codigoUsuario);

        void RemoverCookie(string chave);
    }
}