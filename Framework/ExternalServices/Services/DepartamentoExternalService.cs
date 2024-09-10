using Atron.Application.DTO;
using Atron.Domain.ApiEntities;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Enums;
using Shared.Models;

namespace ExternalServices.Services
{
    /// <summary>
    /// Classe que implementa o processo e fluxo de departamentos
    /// </summary>
    public class DepartamentoExternalService : IDepartamentoExternalService
    {
        private readonly IApiClient _apiClient;
        private readonly ICommunicationService _communicationService;
        private readonly IApiRouteExternalService _apiRouteExternalService;

        public List<ResultResponseDTO> ResultResponses { get; set; }


        public string Uri { get; set; }

        public string Modulo { get; set; }

        public DepartamentoExternalService(IApiClient apiClient, ICommunicationService communicationService, IApiRouteExternalService apiRouteExternalService)
        {
            _apiRouteExternalService = apiRouteExternalService;
            _apiClient = apiClient;
            _communicationService = communicationService;
        }



        public async Task Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            var json = JsonConvert.SerializeObject(departamentoDTO);
            try
            {
                await _apiClient.PutAsync("https://atron-hmg.azurewebsites.net/api/Departamento/AtualizarDepartamento/", codigo, json);
                ResultResponses.AddRange(_communicationService.GetResultResponses());
            }
            catch (HttpRequestException ex)
            {
                var errorResponse = JsonConvert.DeserializeObject<List<ResultResponseDTO>>(ex.Message);
                ResultResponses.AddRange(errorResponse);
            }
            catch (Exception ex)
            {
                ResultResponses.Add(new ResultResponseDTO() { Message = ex.Message, Level = ResultResponseLevelEnum.Error });
            }
        }

        public async Task Criar(DepartamentoDTO departamento)
        {
            var routes = await _apiRouteExternalService.MontarRotaDoModulo(Uri, Modulo);
            string uri = routes.BuildUriRoute();

            var json = JsonConvert.SerializeObject(departamento);
            await _apiClient.PostAsync(uri, json);
        }

        public async Task<List<DepartamentoDTO>> ObterTodos()
        {
            var rotasDepartamento = await _apiRouteExternalService.MontarRotaDoModulo(Uri, Modulo);
            string uri = rotasDepartamento.BuildUriRoute();

            var response = await _apiClient.GetAsync(uri);

            var departamentos = JsonConvert.DeserializeObject<List<DepartamentoDTO>>(response);
            return departamentos is not null ? departamentos : new List<DepartamentoDTO>();
        }

        public async Task Remover(string codigo)
        {
            await _apiClient.DeleteAsync("https://atron-hmg.azurewebsites.net/api/Departamento/ExcluirDepartamento/", codigo);
            ResultResponses.AddRange(_communicationService.GetResultResponses());
        }

        public IList<Message> GetMessages()
        {
            return _communicationService.GetMessages();
        }
    }

    public static class BuildModuleRouting
    {
        public static string BuildUriRoute(this ApiRoute route)
        {            
            return route is null ? null : $"{route.Url}/{route.Modulo}";
        }
    }
}