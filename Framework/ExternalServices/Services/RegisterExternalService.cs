using Atron.Application.DTO.ApiDTO;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Newtonsoft.Json;
using Shared.Models;

namespace ExternalServices.Services
{
    public class RegisterExternalService : IRegisterExternalService
    {
        private readonly IUrlTransferService _urlModuleFactory;
        private readonly MessageModel _messageModel;
        private readonly IApiClient _apiClient;

        public RegisterExternalService(
            IUrlTransferService urlModuleFactory,
            MessageModel messageModel,
            IApiClient apiClient)
        {
            _urlModuleFactory = urlModuleFactory;
            _apiClient = apiClient;
            _messageModel = messageModel;
        }

        public async Task Registrar(RegisterDTO registerDTO)
        {
            var json = JsonConvert.SerializeObject(registerDTO);
            await _apiClient.PostAsync(json);
            // _messageModel.Messages.AddMessages(_communicationService);
        }
    }
}