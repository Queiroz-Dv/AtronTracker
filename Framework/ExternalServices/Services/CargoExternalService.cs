using Atron.Application.DTO;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Newtonsoft.Json;
using Shared.DTO;

namespace ExternalServices.Services
{
    public class CargoExternalService : ICargoExternalService
    {
        private readonly IApiClient _apiClient;
        private readonly ICommunicationService _communicationService;
        public List<ResultResponse> ResultResponses { get; set; }

        public CargoExternalService(IApiClient apiClient, ICommunicationService communicationService)
        {
            _apiClient = apiClient;
            _communicationService = communicationService;
            ResultResponses = new List<ResultResponse>();
        }

        public async Task Criar(CargoDTO cargoDTO)
        {
            var json = JsonConvert.SerializeObject(cargoDTO);
            await _apiClient.PostAsync("https://atron-hmg.azurewebsites.net/api/Cargo/CriarCargo", json);
            ResultResponses.AddRange(_communicationService.GetResultResponses());
        }

        public async Task<List<CargoDTO>?> ObterTodos()
        {
            var response = await _apiClient.GetAsync("https://atron-hmg.azurewebsites.net/api/Cargo/ObterCargos");

            var cargos = JsonConvert.DeserializeObject<List<CargoDTO>>(response);

            return cargos is not null? cargos : null;
        }
    }
}