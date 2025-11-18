using Microsoft.AspNetCore.Http;
using Shared.DTO.API;

namespace Shared.Application.Interfaces.Service
{
    public interface ICookieService
    {
        Task<DadosDoTokenDTO> ObterTokenRefreshTokenPorRequest(HttpRequest httpRequest);

        void CriarCookieDoToken(DadosDoTokenDTO dadosDoToken, string codigoUsuario);

        void RemoverCookie(string chave);
    }
}