using Microsoft.AspNetCore.Http;
using Shared.DTO.API;

namespace Shared.Interfaces.Factory
{
    public interface ICookieFactory
    {
        DadosDeTokenComRefreshToken ObterDadosDoTokenPorRequest(HttpRequest request);

        void CriarCookieDoToken(DadosDoTokenDTO dadosDoToken, string codigoUsuario);

        void RemoverCookie(string chave);
    }
}