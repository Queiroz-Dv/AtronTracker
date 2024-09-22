using Atron.Domain.ApiEntities;

namespace Atron.Application.ApiInterfaces
{
    /// <summary>
    /// Interface com os processos do módulo de Rotas
    /// </summary>
    public interface IApiRouteService
    {
        ApiRoute ObterRotaPorModulo(string modulo);
    }
}