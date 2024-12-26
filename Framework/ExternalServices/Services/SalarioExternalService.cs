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

        public Task Atualizar(string id, SalarioDTO salarioDTO)
        {
            throw new NotImplementedException();
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

        public Task<List<SalarioDTO>> ObterTodos()
        {
            throw new NotImplementedException();
        }

        public Task Remover(string id)
        {
            throw new NotImplementedException();
        }
    }
}
