using Atron.Application.DTO;
using Communication.Extensions;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Enums;
using Shared.Extensions;

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


        public async Task Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            var json = JsonConvert.SerializeObject(departamentoDTO);
            try
            {
                var response = await _apiClient.PutAsync("https://atron-hmg.azurewebsites.net/api/Departamento/AtualizarDepartamento/", codigo, json);
                ResultResponses.AddRange(_communicationService.GetResultResponses());
            }
            catch (HttpRequestException ex)
            {
                var errorResponse = JsonConvert.DeserializeObject<List<ResultResponse>>(ex.Message);
                ResultResponses.AddRange(errorResponse);
            }
            catch (Exception ex)
            {
                ResultResponses.Add(new ResultResponse() { Message = ex.Message, Level = ResultResponseLevelEnum.Error.GetEnumDescription() });
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

        public async Task Remover(string codigo)
        {
            var response = await _apiClient.DeleteAsync("https://atron-hmg.azurewebsites.net/api/Departamento/ExcluirDepartamento/", codigo);
            ResultResponses.AddRange(_communicationService.GetResultResponses());
        }
    }
}