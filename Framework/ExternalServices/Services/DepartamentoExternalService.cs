using Atron.Application.DTO;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Extensions;
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
        private readonly IUrlModuleFactory _urlFactory;
        private readonly IApiClient _apiClient;
        private readonly ICommunicationService _communicationService;
        private readonly IApiRouteExternalService _apiRouteExternalService;
        private readonly MessageModel<Departamento> _messageModel;

        public string Uri { get; set; }

        public string Modulo { get; set; }

        public DepartamentoExternalService(
            IUrlModuleFactory urlFactory,
            IApiClient apiClient,
            ICommunicationService communicationService,
            IApiRouteExternalService apiRouteExternalService,
            MessageModel<Departamento> messageModel)
        {
            _urlFactory = urlFactory;
            _apiRouteExternalService = apiRouteExternalService;
            _apiClient = apiClient;
            _communicationService = communicationService;
            _messageModel = messageModel;
        }

        public async Task Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            var json = JsonConvert.SerializeObject(departamentoDTO);

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(departamentoDTO.Codigo))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Departamento));
                //_communicationService.AddMessage(new Message() { Description = "Código não informado", Level = MessageLevel.Error });
                return;
            }

            try
            {
                var routa = await _apiRouteExternalService.MontarRotaDoModulo(Uri, Modulo);
                string uri = routa.BuildUri();
                uri += $"/{string.Empty}";

                await _apiClient.PutAsync(uri, codigo, json);
            }
            catch (HttpRequestException ex)
            {
                var exceptionMessage = JsonConvert.DeserializeObject<List<Message>>(ex.Message);
                _communicationService.AddMessages(exceptionMessage);
            }
            catch (Exception ex)
            {
                _communicationService.AddMessage(new Message() { Description = ex.Message, Level = MessageLevel.Error });
            }
        }

        public async Task<DepartamentoDTO> ObterPorCodigo(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Departamento));
                return new DepartamentoDTO();
            }

            var rotaDepartamento = await _apiRouteExternalService.MontarRotaDoModulo(Uri, Modulo);
            string uri = rotaDepartamento.BuildUri();
            //melhorar essa parte
            uri += $"/{codigo}";
            var response = await _apiClient.GetAsync(uri);
            var departamento = JsonConvert.DeserializeObject<DepartamentoDTO>(response);
            return departamento;
        }

        public async Task Criar(DepartamentoDTO departamento)
        {
            var json = JsonConvert.SerializeObject(departamento);
            await _apiClient.PostAsync(_urlFactory.Url, json);
        }

        public async Task<List<DepartamentoDTO>> ObterTodos()
        {
            var response = await _apiClient.GetAsync(_urlFactory.Url);

            var departamentos = JsonConvert.DeserializeObject<List<DepartamentoDTO>>(response);
            return departamentos is not null ? departamentos : new List<DepartamentoDTO>();
        }

        public async Task Remover(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Departamento));
                return;
            }

            var rotaDepartamento = await _apiRouteExternalService.MontarRotaDoModulo(Uri, Modulo);
            string uri = rotaDepartamento.BuildUri();
            await _apiClient.DeleteAsync(uri, codigo);
        }
    }
}