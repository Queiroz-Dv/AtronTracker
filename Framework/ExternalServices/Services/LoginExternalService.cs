using Atron.Application.DTO.ApiDTO;
using Communication.Interfaces;
using ExternalServices.Interfaces;
using Newtonsoft.Json;

namespace ExternalServices.Services
{
    public class LoginExternalService : ILoginExternalService
    {
        private readonly IApiClient _apiClient;

        public LoginExternalService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<LoginDTO> Autenticar(LoginDTO loginDTO)
        {
            var json = JsonConvert.SerializeObject(loginDTO);

            var loginResult = await _apiClient.PostAsync<LoginDTO>(json);

            return loginResult;
        }

        public async Task Logout()
        {
            await _apiClient.GetAsync();
        }
    }
}