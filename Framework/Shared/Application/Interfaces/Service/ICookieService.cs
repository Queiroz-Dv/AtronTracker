using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Auth;

namespace Shared.Application.Interfaces.Service
{
    public interface ICookieService
    {
        Task<DadosDoRefreshTokenCookieDTO> ObterRefreshTokenPorRequest(HttpRequest httpRequest);

        void CriarCookieDeRefreshToken(DadosDoRefrehTokenDTO dadosDoRefreshToken, string codigoUsuario);

        void RemoverCookie(string chave);
    }
}
