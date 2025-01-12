using Atron.Domain.ApiEntities;
using Communication.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Newtonsoft.Json;

namespace ExternalServices.Services.ApiRouteServices
{
    /// <summary>
    /// Classe que implementa os processos do fluxo do módulo de Rotas da API
    /// </summary>
    public class ApiRouteExternalService : IApiRouteExternalService
    {
        private readonly IApiClient _apiClient;

        public ApiRouteExternalService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ApiRoute> MontarRotaDoModulo(string rota, string modulo)
        {
            var response = await _apiClient.GetAsync(rota, modulo);
            var rotas = JsonConvert.DeserializeObject<ApiRoute>(response);
            return rotas;
        }
    }
}