using Atron.Application.DTO;
using Communication.Interfaces.Services;
using Communication.Interfaces;
using ExternalServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public async Task<List<CargoDTO>> ObterTodos()
        {
            var response = await _apiClient.GetAsync("https://atron-hmg.azurewebsites.net/api/Cargo/ObterCargos");

            var cargos = JsonConvert.DeserializeObject<List<CargoDTO>>(response);

            return cargos;
        }
    }
}
