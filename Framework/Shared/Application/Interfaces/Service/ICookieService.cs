using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Auth;

namespace Shared.Application.Interfaces.Service
{
    public interface ICookieService
    {
        Task<DadosDoTokenDTO> ObterTokenRefreshTokenPorRequest(HttpRequest httpRequest);

        void CriarCookieDoToken(DadosDoTokenDTO dadosDoToken, string codigoUsuario);

        void RemoverCookie(string chave);
    }
}