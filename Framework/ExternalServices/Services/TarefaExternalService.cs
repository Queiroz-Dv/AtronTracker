using Atron.Application.DTO;
using Atron.Domain.Entities;
using Communication.Interfaces.Services;
using Communication.Interfaces;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Shared.Models;
using Newtonsoft.Json;
using Communication.Extensions;

namespace ExternalServices.Services
{
    public class TarefaExternalService : ITarefaExternalService
    {
        private readonly IUrlModuleFactory _urlModuleFactory;
        private readonly IApiClient _apiClient;
        private readonly ICommunicationService _communicationService;
        private MessageModel<Tarefa> _messageModel;

        public TarefaExternalService(
            IUrlModuleFactory urlModuleFactory,
            IApiClient apiClient,
            ICommunicationService communicationService,
            MessageModel<Tarefa> messageModel)
        {
            _urlModuleFactory = urlModuleFactory;
            _apiClient = apiClient;
            _communicationService = communicationService;
            _messageModel = messageModel;
        }

        public async Task Atualizar(string id, TarefaDTO tarefaDTO)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Tarefa));
                return;
            }

            var tarefaJson = JsonConvert.SerializeObject(tarefaDTO);

            try
            {
                await _apiClient.PutAsync(_urlModuleFactory.Url, id, tarefaJson);
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

        public async Task Criar(TarefaDTO tarefaDTO)
        {
            var json = JsonConvert.SerializeObject(tarefaDTO);
            await _apiClient.PostAsync(_urlModuleFactory.Url, json);
            _messageModel.Messages.AddMessages(_communicationService);
        }

        public async Task<TarefaDTO> ObterPorId()
        {
            var response = await _apiClient.GetAsync(_urlModuleFactory.Url);
            return JsonConvert.DeserializeObject<TarefaDTO>(response);
        }

        public async Task<List<TarefaDTO>> ObterTodos()
        {
            var response = await _apiClient.GetAsync(_urlModuleFactory.Url);

            return JsonConvert.DeserializeObject<List<TarefaDTO>>(response);
        }

        public async Task Remover(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Tarefa));
                return;
            }

            await _apiClient.DeleteAsync(_urlModuleFactory.Url, id);
            _messageModel.Messages.AddMessages(_communicationService);
        }
    }
}