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
    public class SalarioExternalService : ISalarioExternalService
    {
        private readonly IUrlModuleFactory _urlModuleFactory;
        private readonly IApiClient _apiClient;
        private readonly ICommunicationService _communicationService;
        private MessageModel<Salario> _messageModel;

        public SalarioExternalService(
            IUrlModuleFactory urlModuleFactory,
            IApiClient apiClient,
            ICommunicationService communicationService,
            MessageModel<Salario> messageModel)
        {
            _urlModuleFactory = urlModuleFactory;
            _apiClient = apiClient;
            _communicationService = communicationService;
            _messageModel = messageModel;
        }

        public async Task Atualizar(string id, SalarioDTO salarioDTO)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Salario));
                return;
            }

            var salarioJSon = JsonConvert.SerializeObject(salarioDTO);

            try
            {
                await _apiClient.PutAsync(_urlModuleFactory.Url, id, salarioJSon);
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
            catch (JsonSerializationException ex)
            {
                _messageModel.AddError(ex.Message);
            }
            catch (Exception ex)
            {
                _messageModel.AddError(ex.Message);
            }
        }

        public async Task Criar(SalarioDTO salarioDTO)
        {
            var json = JsonConvert.SerializeObject(salarioDTO);
            try
            {
                await _apiClient.PostAsync(_urlModuleFactory.Url, json);
                _messageModel.Messages.AddMessages(_communicationService);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<List<MesDTO>> ObterMeses()
        {
            var response = await _apiClient.GetAsync(_urlModuleFactory.Url);

            return JsonConvert.DeserializeObject<List<MesDTO>>(response);
        }

        public async Task<SalarioDTO> ObterPorId()
        {
            var response = await _apiClient.GetAsync(_urlModuleFactory.Url);
            return JsonConvert.DeserializeObject<SalarioDTO>(response);
        }

        public async Task<List<SalarioDTO>> ObterTodos()
        {
            var response = await _apiClient.GetAsync(_urlModuleFactory.Url);

            return JsonConvert.DeserializeObject<List<SalarioDTO>>(response);
        }

        public Task Remover(string id)
        {
            throw new NotImplementedException();
        }
    }
}
