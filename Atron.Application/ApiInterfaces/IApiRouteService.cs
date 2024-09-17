using Atron.Domain.ApiEntities;
using System.Collections.Generic;

namespace Atron.Application.ApiInterfaces
{
    /// <summary>
    /// Interface com os processos do módulo de Rotas
    /// </summary>
    public interface IApiRouteService
    {
        /// <summary>
        /// Obtém todas as rotas da API
        /// </summary>
        List<ApiRoute> MontarRotasPorModuloService(string modulo);

        ApiRoute ObterRotaPorModulo(string modulo);
    }
}