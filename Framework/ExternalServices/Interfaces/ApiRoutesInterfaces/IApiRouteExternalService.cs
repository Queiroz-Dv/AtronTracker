using Atron.Domain.ApiEntities;
using ExternalServices.Interfaces.ExternalMessage;

namespace ExternalServices.Interfaces.ApiRoutesInterfaces
{
    public interface IApiRouteExternalService : IExternalMessageService
    {
        Task Cadastrar(ApiRoute route, string rotaDoConnect);
        Task<List<ApiRoute>> ObterRotas(string rotaPrincipal);
    }
}