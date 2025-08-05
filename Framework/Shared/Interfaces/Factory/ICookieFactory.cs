using Microsoft.AspNetCore.Http;
using Shared.DTO.API;

namespace Shared.Interfaces.Factory
{
    public interface ICookieFactory
    {
        Task<DadosDoTokenDTO> ObterDadosDoTokenPorRequest(HttpRequest request);

        void CriarCookieDoToken(DadosDoTokenDTO dadosDoToken, string codigoUsuario);

        void RemoverCookie(string chave);
    }
}