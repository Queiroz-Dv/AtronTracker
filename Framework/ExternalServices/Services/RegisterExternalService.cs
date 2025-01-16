using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using Communication.Extensions;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Newtonsoft.Json;
using Shared.Models;

namespace ExternalServices.Services
{
    public class RegisterExternalService : IRegisterExternalService
    {
        private readonly IUrlModuleFactory _urlModuleFactory;
        private readonly ICommunicationService _communicationService;
        private readonly MessageModel<ApiRegister> _messageModel;
        private readonly IApiClient _apiClient;

        public RegisterExternalService(
            IUrlModuleFactory urlModuleFactory,
            ICommunicationService communicationService,
            MessageModel<ApiRegister> messageModel,
            IApiClient apiClient)
        {
            _urlModuleFactory = urlModuleFactory;
            _apiClient = apiClient;
            _communicationService = communicationService;
            _messageModel = messageModel;
        }

        public async Task Registrar(RegisterDTO registerDTO)
        {
            var json = JsonConvert.SerializeObject(registerDTO);
            await _apiClient.PostAsync(_urlModuleFactory.Url, json);
            _messageModel.Messages.AddMessages(_communicationService);
        }
    }
}