using Microsoft.AspNetCore.Http;
using Shared.DTO.API;

namespace Shared.Interfaces.Handlers
{
    public interface ICacheHandlerService
    {
        InfoToken ObterInfoTokensDoCachePorRequest(HttpRequest request);
    }
}