using Atron.Application.DTO;
using Atron.Domain.Entities;
using Communication.Extensions;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Newtonsoft.Json;
using Shared.Models;

namespace ExternalServices.Services
{
    /// <summary>
    /// Classe que implementa o fluxo de processos do módulo de cargos
    /// </summary>
    public class CargoExternalService : ICargoExternalService
    {
        private readonly IUrlModuleFactory _urlModuleFactory;
        private readonly IApiClient _apiClient;
        private readonly ICommunicationService _communicationService;
        private MessageModel<Cargo> _messageModel;

        public CargoExternalService(
            IUrlModuleFactory urlModuleFactory,
            IApiClient apiClient,
            ICommunicationService communicationService,
            MessageModel<Cargo> messageModel)
        {
            _urlModuleFactory = urlModuleFactory;
            _apiClient = apiClient;
            _communicationService = communicationService;
            _messageModel = messageModel;
        }

        public async Task Criar(CargoDTO cargoDTO)
        {
            RemontarEntidade(cargoDTO);

            var json = JsonConvert.SerializeObject(cargoDTO);
            await _apiClient.PostAsync(_urlModuleFactory.Url, json);
            _messageModel.Messages.AddMessages(_communicationService);
        }

        private static void RemontarEntidade(CargoDTO cargoDTO)
        {
            cargoDTO.Codigo = cargoDTO.Codigo.ToUpper();
            cargoDTO.Descricao = cargoDTO.Descricao.ToUpper();
        }

        public async Task<List<CargoDTO>> ObterTodos()
        {
            var response = await _apiClient.GetAsync(_urlModuleFactory.Url);

            var cargos = JsonConvert.DeserializeObject<List<CargoDTO>>(response);
            return cargos is not null ? cargos : new List<CargoDTO>();
        }

        public async Task Atualizar(string codigo, CargoDTO cargoDTO)
        {
            RemontarEntidade(cargoDTO);

            var json = JsonConvert.SerializeObject(cargoDTO);

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(cargoDTO.Codigo))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Cargo));
                return;
            }

            try
            {
                await _apiClient.PutAsync(_urlModuleFactory.Url, codigo, json);
                _messageModel.Messages.AddMessages(_communicationService);
            }
            catch (HttpRequestException ex)
            {
                var errorResponse = JsonConvert.DeserializeObject<List<Message>>(ex.Message);
                _messageModel.Messages.AddRange(errorResponse);
            }
            catch (Exception ex)
            {
                _messageModel.AddError(ex.Message);
            }
        }

        public async Task Remover(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Cargo));
                return;
            }

            await _apiClient.DeleteAsync(_urlModuleFactory.Url, codigo);
            _messageModel.Messages.AddMessages(_communicationService);
        }

        public async Task<CargoDTO>? ObterPorCodigo(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Cargo));
                return null;
            }

            var response = await _apiClient.GetAsync(_urlModuleFactory.Url);
            return JsonConvert.DeserializeObject<CargoDTO>(response);

        }
    }
}