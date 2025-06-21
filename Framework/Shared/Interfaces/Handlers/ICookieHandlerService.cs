using Microsoft.AspNetCore.Http;
using Shared.DTO.API;

namespace Shared.Interfaces.Handlers
{
    public interface ICookieHandlerService
    {
        InfoToken ExtrairInfoTokensDoCookie(HttpRequest request);

        void CriarCookiesDoToken(InfoToken userInfo);
    }
}