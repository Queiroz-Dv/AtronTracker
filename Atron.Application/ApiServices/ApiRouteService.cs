using Atron.Application.ApiInterfaces;
using Atron.Domain.ApiEntities;
using Atron.Domain.ApiInterfaces;
using Notification.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices
{
    public class ApiRouteService : IApiRouteService
    {
        private IApiRouteRepository _routeRepository;

        public ApiRouteService(IApiRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
            Messages = new List<NotificationMessage>();
        }

        public List<NotificationMessage> Messages { get; set; }

        public Task<IEnumerable<ApiRoute>> ObterTodasRotasServiceAsync()
        {
            var rotas = _routeRepository.ObterTodasRotas();
            return rotas;
        }
    }
}
