using Microsoft.AspNetCore.Http;
using Shared.DTO.API;

namespace Shared.Application.Interfaces.Service
{
    public interface ICookieFactoryService
    {
        Task<DadosDoTokenDTO> ObterDadosDoTokenPorRequest(HttpRequest request);

        void CriarCookieDoToken(DadosDoTokenDTO dadosDoToken, string codigoUsuario);

        void RemoverCookie(string chave);
    }
}