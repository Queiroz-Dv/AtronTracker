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

        public async Task CriarRotaAsync(ApiRoute apiRoute)
        {
            var entity = new ApiRoute();
            entity.Id = apiRoute.Id;
            entity.Modulo = apiRoute.Modulo;
            entity.Ativo = apiRoute.Ativo;
            entity.Acao = apiRoute.Acao;

            await _routeRepository.CriarRotaRepositoryAsync(entity);
            Messages.Add(new NotificationMessage("Rota criada com sucesso."));
        }

        public Task<IEnumerable<ApiRoute>> ObterTodasRotasServiceAsync()
        {
            var rotas = _routeRepository.ObterTodasRotas();
            return rotas;
        }
    }
}
