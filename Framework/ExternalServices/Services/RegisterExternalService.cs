using Atron.Application.DTO.ApiDTO;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Newtonsoft.Json;
using Shared.Models;

namespace ExternalServices.Services
{
    public class RegisterExternalService : ExternalService<UsuarioRegistroDTO>, IRegisterExternalService
    {
        private readonly IApiClient _apiClient;

        public RegisterExternalService(
            IUrlTransferService urlModuleFactory,
            MessageModel messageModel,
            IApiClient apiClient) : base(apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<bool> EmailExiste(string email)
        {
            var dto = await _apiClient.GetAsync<UsuarioRegistroDTO>(email);
            return dto is not null;
        }

        public async Task Registrar(UsuarioRegistroDTO registerDTO)
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