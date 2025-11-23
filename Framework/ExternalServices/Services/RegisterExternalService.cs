using Application.DTO.ApiDTO;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Newtonsoft.Json;
using Shared.Domain.ValueObjects;

namespace ExternalServices.Services
{
    public class RegisterExternalService : ExternalService<RegisterDTO>, IRegisterExternalService
    {
        private readonly IApiClient _apiClient;

        public RegisterExternalService(
            IUrlTransferService urlModuleFactory,
            Notifiable messageModel,
            IApiClient apiClient) : base(apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<bool> EmailExiste(string email)
        {
            var dto = await _apiClient.GetAsync<RegisterDTO>(email);
            return dto is not null;
        }

        public async Task Registrar(RegisterDTO registerDTO)
        {
            var json = JsonConvert.SerializeObject(registerDTO);
            await _apiClient.PostAsync(json);
        }

        public async Task<bool> UsuarioExiste(string codigo)
        {
            var dto = await ObterPorCodigo(codigo);
            return dto is not null;
        }
    }
}