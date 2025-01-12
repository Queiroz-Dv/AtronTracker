using Atron.Application.DTO.ApiDTO;
using Communication.Interfaces.Services;
using Communication.Interfaces;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Newtonsoft.Json;

namespace ExternalServices.Services
{
    public class LoginExternalService : ILoginExternalService
    {
        private readonly IUrlModuleFactory _urlModuleFactory;
        private readonly IApiClient _apiClient;        

        public LoginExternalService(
            IUrlModuleFactory urlModuleFactory,
            IApiClient apiClient, 
            ICommunicationService communicationService)
        {
            _urlModuleFactory = urlModuleFactory;
            _apiClient = apiClient;            
        }

        public async Task<LoginDTO> Autenticar(LoginDTO loginDTO)
        {
            var json = JsonConvert.SerializeObject(loginDTO);

            var loginResult = await _apiClient.PostAsync<LoginDTO>(_urlModuleFactory.Url, json);

            return loginResult;
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }       
    }
}