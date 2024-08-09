using Atron.Application.DTO;
using Communication.Extensions;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Newtonsoft.Json;
using Shared.DTO;

namespace ExternalServices.Services
{
    public class DepartamentoExternalService : IDepartamentoExternalService
    {
        private readonly IApiClient _apiClient;
        private readonly ICommunicationService _communicationService;
        public List<ResultResponse> ResultResponses { get; set; }

        public DepartamentoExternalService(IApiClient apiClient, ICommunicationService communicationService)
        {
            _apiClient = apiClient;
            _communicationService = communicationService;
            ResultResponses = new List<ResultResponse>();
        }


        public async Task<(bool isSucess, List<ResultResponse> responses)> Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            var json = JsonConvert.SerializeObject(departamentoDTO);
            try
            {
                var response = await _apiClient.PutAsync("https://atron-hmg.azurewebsites.net/api/Departamento/AtualizarDepartamento/", codigo, json);
                var notifications = _communicationService.GetResultResponses();
                return (true, notifications);
            }
            catch (HttpRequestException ex)
            {
                var errorResponse = JsonConvert.DeserializeObject<List<ResultResponse>>(ex.Message);
                return (false, errorResponse);
                throw;
            }
            catch (Exception ex)
            {
                return (false, new List<ResultResponse> { new ResultResponse { Message = ex.Message } });
            }

        }

        public async Task Criar(DepartamentoDTO departamento)
        {
            var json = JsonConvert.SerializeObject(departamento);
            await _apiClient.PostAsync("https://atron-hmg.azurewebsites.net/api/Departamento/CriarDepartamento", json);
            ResultResponses.AddRange(_communicationService.GetResultResponses());
        }

        public async Task<List<DepartamentoDTO>> ObterTodos()
        {
            var response = await _apiClient.GetAsync("https://atron-hmg.azurewebsites.net/api/Departamento/ObterDepartamentos");
            return JsonConvert.DeserializeObject<List<DepartamentoDTO>>(response);
        }

        public async Task<(bool isSuccess, List<ResultResponse> responses)> Remover(string codigo)
        {
            var response = await _apiClient.DeleteAsync("https://atron-hmg.azurewebsites.net/api/Departamento/ExcluirDepartamento/", codigo);
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
    }
}