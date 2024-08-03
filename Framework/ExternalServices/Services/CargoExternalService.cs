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

        public CargoExternalService(IApiClient apiClient, ICommunicationService communicationService)
        {
            _apiClient = apiClient;
            _communicationService = communicationService;
        }

        public async Task<(bool isSucess, List<ResultResponse> responses)> Criar(CargoDTO cargoDTO)
        {            
            var json  = JsonConvert.SerializeObject(cargoDTO);
            var response = await _apiClient.PostAsync("https://atron-hmg.azurewebsites.net/api/Cargo/CriarCargo", json);
            var notifications = _communicationService.GetResultResponses();
            return (true, notifications);
        }

        public async Task<List<CargoDTO>> ObterTodos()
        {
            var response = await _apiClient.GetAsync("https://atron-hmg.azurewebsites.net/api/Cargo/ObterCargos");

            var cargos = JsonConvert.DeserializeObject<List<CargoDTO>>(response);

            return cargos;
        }
    }
}
