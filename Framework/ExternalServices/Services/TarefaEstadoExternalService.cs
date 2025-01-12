using Atron.Domain.Entities;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Newtonsoft.Json;

namespace ExternalServices.Services
{
    public class TarefaEstadoExternalService : ITarefaEstadoExternalService
    {
        private readonly IUrlModuleFactory _urlModuleFactory;
        private readonly IApiClient _client;
        //private readonly ICommunicationService _communicationService;

        public TarefaEstadoExternalService(
            IUrlModuleFactory urlModuleFactory,
            IApiClient client)
        {
            _urlModuleFactory = urlModuleFactory;
            _client = client;
        }

        public async Task<List<TarefaEstado>> ObterTodosAsync()
        {
            var response = await _client.GetAsync(_urlModuleFactory.Url);
            var estados = JsonConvert.DeserializeObject<List<TarefaEstado>>(response);
            return estados;
        }
    }
}
