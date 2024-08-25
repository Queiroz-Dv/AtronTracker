﻿using Atron.Application.DTO;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Enums;
using Shared.Extensions;

namespace ExternalServices.Services
{
    public class CargoExternalService : ICargoExternalService
    {
        private readonly IApiClient _apiClient;
        private readonly ICommunicationService _communicationService;

        public List<ResultResponseDTO> ResultResponses { get; set; }

        public CargoExternalService(IApiClient apiClient, ICommunicationService communicationService)
        {
            _apiClient = apiClient;
            _communicationService = communicationService;
            ResultResponses = new List<ResultResponseDTO>();
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

        public async Task Atualizar(string codigo, CargoDTO cargoDTO)
        {
            var json = JsonConvert.SerializeObject(cargoDTO);
            try
            {
                await _apiClient.PutAsync("https://atron-hmg.azurewebsites.net/api/Cargo/AtualizarCargo/", codigo, json);
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

        public async Task Remover(string codigo)
        {
            await _apiClient.DeleteAsync("https://atron-hmg.azurewebsites.net/api/Cargo/ExcluirCargo/", codigo);
            ResultResponses.AddRange(_communicationService.GetResultResponses());
        }
    }
}