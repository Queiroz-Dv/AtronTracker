using Atron.Domain.ApiEntities;
using Notification.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.ApiInterfaces
{
    public interface IApiRouteService
    {
        public List<NotificationMessage> Messages { get; }
        Task CriarRotaAsync(ApiRoute apiRoute);
        Task<IEnumerable<ApiRoute>> ObterTodasRotasServiceAsync();
    }
}