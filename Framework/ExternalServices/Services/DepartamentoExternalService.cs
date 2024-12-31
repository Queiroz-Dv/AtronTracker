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
    /// Classe que implementa o processo e fluxo de departamentos
    /// </summary>
    public class DepartamentoExternalService : IDepartamentoExternalService
    {
        private readonly IUrlModuleFactory _urlFactory;
        private readonly IApiClient _apiClient;
        private readonly ICommunicationService _communicationService;
        private readonly MessageModel<Departamento> _messageModel;

        public DepartamentoExternalService(
            IUrlModuleFactory urlFactory,
            IApiClient apiClient,
            ICommunicationService communicationService,
            MessageModel<Departamento> messageModel)
        {
            _urlFactory = urlFactory;
            _apiClient = apiClient;
            _communicationService = communicationService;
            _messageModel = messageModel;
        }

        public async Task Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(departamentoDTO.Codigo))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Departamento));
                return;
            }

            RemontarEntidade(departamentoDTO);

            var json = JsonConvert.SerializeObject(departamentoDTO);

            try
            {
                await _apiClient.PutAsync(_urlFactory.Url, codigo, json);
                _messageModel.Messages.AddMessages(_communicationService);
            }
            catch (HttpRequestException ex)
            {
                var exceptionMessage = JsonConvert.DeserializeObject<List<Message>>(ex.Message);
                if (exceptionMessage is not null)
                {
                    _messageModel.Messages.AddRange(exceptionMessage);
                }
                else
                {
                    _messageModel.AddError(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _messageModel.AddError(ex.Message);
            }
        }

        public async Task<DepartamentoDTO> ObterPorCodigo(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Departamento));
                return new DepartamentoDTO();
            }

            var response = await _apiClient.GetAsync(_urlFactory.Url);
            return JsonConvert.DeserializeObject<DepartamentoDTO>(response);
        }

        public async Task Criar(DepartamentoDTO departamento)
        {
            RemontarEntidade(departamento);

            var json = JsonConvert.SerializeObject(departamento);
            await _apiClient.PostAsync(_urlFactory.Url, json);
            _messageModel.Messages.AddMessages(_communicationService);
        }

        private static void RemontarEntidade(DepartamentoDTO departamento)
        {
            departamento.Codigo = departamento.Codigo.ToUpper();
            departamento.Descricao = departamento.Descricao.ToUpper();
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

            await _apiClient.DeleteAsync(_urlFactory.Url, codigo);
            _messageModel.Messages.AddMessages(_communicationService);
        }
    }
}