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
    /// Classe que implementa os processos e fluxo do módulo de Usuários
    /// </summary>
    public class UsuarioExternalService : IUsuarioExternalService
    {
        private readonly IApiClient _client;
        private readonly ICommunicationService _communicationService;
        private readonly IUrlModuleFactory _urlModuleFactory;
        private MessageModel<Usuario> _messageModel;

        public UsuarioExternalService(IApiClient apiClient,
            ICommunicationService communicationService,
            IUrlModuleFactory urlModuleFactory,
            MessageModel<Usuario> messageModel)
        {
            _client = apiClient;
            _communicationService = communicationService;
            _urlModuleFactory = urlModuleFactory;
            _messageModel = messageModel;
        }

        public async Task<List<UsuarioDTO>> ObterTodos()
        {
            var response = await _client.GetAsync(_urlModuleFactory.Url);

            return JsonConvert.DeserializeObject<List<UsuarioDTO>>(response);
        }

        public async Task Criar(UsuarioDTO model)
        {
            var json = JsonConvert.SerializeObject(model);
            await _client.PostAsync(_urlModuleFactory.Url, json);
            _messageModel.Messages.AddMessages(_communicationService);
        }
    }
}