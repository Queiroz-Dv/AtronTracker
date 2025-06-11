using Microsoft.AspNetCore.Http;
using Shared.DTO.API;

namespace Shared.Interfaces.Handlers
{
    public interface ICookieHandlerService
    {
        InfoToken ExtrairTokensDoCookie(HttpRequest request);

        void CriarCookiesDoToken(InfoToken userInfo);
    }
}