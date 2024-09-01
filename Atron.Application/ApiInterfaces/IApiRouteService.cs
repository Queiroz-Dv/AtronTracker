using Atron.Domain.ApiEntities;
using Notification.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.ApiInterfaces
{
    /// <summary>
    /// Interface com os processos do módulo de Rotas
    /// </summary>
    public interface IApiRouteService
    {
        /// <summary>
        /// Lista de Mensagens que é utilizada no processo para comunicação com outros módulos
        /// </summary>
        public List<NotificationMessage> Messages { get; }

        /// <summary>
        /// Método de criação de uma rota
        /// </summary>
        /// <param name="apiRoute">Entidade que será enviada para criação</param>
        Task CriarRotaAsync(ApiRoute apiRoute);

        /// <summary>
        /// Obtém todas as rotas da API
        /// </summary>
        Task<IEnumerable<ApiRoute>> ObterTodasRotasServiceAsync();
    }
}