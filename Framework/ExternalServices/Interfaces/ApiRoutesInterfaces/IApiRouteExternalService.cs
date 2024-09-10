using Atron.Domain.ApiEntities;
using ExternalServices.Interfaces.ExternalMessage;

namespace ExternalServices.Interfaces.ApiRoutesInterfaces
{
    /// <summary>
    /// Interface dos processos e fluxos do módulo de rotas da API
    /// </summary>
    public interface IApiRouteExternalService
    {       

        /// <summary>
        /// Método que obtém todas as rotas
        /// </summary>
        /// <param name="modulo">Rota de acesso principal</param>
        /// <returns>Retorna todas as rotas disponíveis</returns>
        Task<ApiRoute> MontarRotaDoModulo(string rota, string modulo);
    }
}