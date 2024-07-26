using Atron.Application.DTO;
using Communication.Extensions;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Newtonsoft.Json;
using Shared.DTO;

namespace ExternalServices.Models
{
    public class DepartamentoExternalService : IDepartamentoExternalService
    {
        private readonly IApiClient _apiClient;
        private readonly ICommunicationService _communicationService;

        public DepartamentoExternalService(IApiClient apiClient, ICommunicationService communicationService)
        {
            _apiClient = apiClient;
            _communicationService = communicationService;
        }

        public async Task<(bool isSucess, List<ResultResponse> responses)> CriarDepartamento(DepartamentoDTO departamento)
        {
            var json = JsonConvert.SerializeObject(departamento);
            var response = await _apiClient.PostAsync("https://atron-hmg.azurewebsites.net/api/Departamento/CriarDepartamento", json);
            var notifications = _communicationService.GetResultResponses();

            if (!notifications.HasErrors())
            {
                return (true, notifications);
            }
            else
            {               
                return (false, notifications);
            }
        }

        public async Task<List<DepartamentoDTO>> ObterDepartamentos()
        {
            var response = await _apiClient.GetAsync("https://atron-hmg.azurewebsites.net/api/Departamento/ObterDepartamentos");
            return JsonConvert.DeserializeObject<List<DepartamentoDTO>>(response);
        }
    }
}