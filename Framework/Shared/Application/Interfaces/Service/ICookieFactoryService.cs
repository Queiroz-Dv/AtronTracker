using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Auth;

namespace Shared.Application.Interfaces.Service
{
    public interface ICookieFactoryService
    {
        Task<DadosDoRefreshTokenCookieDTO> ObterRefreshTokenPorRequest(HttpRequest request);

        void CriarCookieDeRefreshToken(DadosDoRefrehTokenDTO dadosDoRefreshToken, string codigoUsuario);

        void RemoverCookie(string chave);
    }
}
