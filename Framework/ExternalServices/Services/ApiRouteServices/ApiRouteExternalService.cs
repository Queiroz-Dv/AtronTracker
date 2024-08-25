using Atron.Domain.ApiEntities;
using Communication.Interfaces.Services;
using Communication.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Shared.DTO;
using Newtonsoft.Json;

namespace ExternalServices.Services.ApiRouteServices
{
    public class ApiRouteExternalService : IApiRouteExternalService
    {
        private readonly IApiClient _apiClient;
        private readonly ICommunicationService _communicationService;

        public ApiRouteExternalService(IApiClient apiClient, ICommunicationService communicationService)
        {
            _apiClient = apiClient;
            _communicationService = communicationService;
            ResultResponses = new List<ResultResponseDTO>();
        }

        public List<ResultResponseDTO> ResultResponses { get; set; }

        public async Task Cadastrar(ApiRoute route, string rotaDoConnect)
        {
            var json = JsonConvert.SerializeObject(route);
            await _apiClient.PostAsync(rotaDoConnect, json);
            ResultResponses.AddRange(_communicationService.GetResultResponses());
        }

        public async Task<List<ApiRoute>> ObterRotas(string rotaPrincipal)
        {
            var response = await _apiClient.GetAsync(rotaPrincipal);
            var rotas = JsonConvert.DeserializeObject<List<ApiRoute>>(response);
            return rotas;
        }
    }
}
