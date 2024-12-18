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

        public async Task Criar(TarefaDTO tarefaDTO)
        {
            var json = JsonConvert.SerializeObject(tarefaDTO);
            await _apiClient.PostAsync(_urlModuleFactory.Url, json);
            _messageModel.Messages.AddMessages(_communicationService);
        }

        public async Task<List<TarefaDTO>> ObterTodos()
        {
            var response = await _apiClient.GetAsync(_urlModuleFactory.Url);

            return JsonConvert.DeserializeObject<List<TarefaDTO>>(response);
        }
    }
}