using Atron.Application.DTO;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using Newtonsoft.Json;
using Shared.DTO;

namespace ExternalServices.Services
{
    public class UsuarioExternalService : IUsuarioExternalService
    {
        private readonly IApiClient _client;
        private readonly ICommunicationService _communicationService;

        public List<ResultResponse> ResultResponses { get; set; }

        public UsuarioExternalService(IApiClient apiClient, ICommunicationService communicationService)
        {
            _client = apiClient;
            _communicationService = communicationService;
            ResultResponses = new List<ResultResponse>();
        }

        public async Task<List<UsuarioDTO>> ObterTodos()
        {
            var response = await _client.GetAsync("https://atron-hmg.azurewebsites.net/api/Usuario/ObterUsuarios");

            var usuarios = JsonConvert.DeserializeObject<List<UsuarioDTO>>(response);

            return usuarios is not null ? usuarios : null;
        }

        public async Task Criar(UsuarioDTO model)
        {
            var json = JsonConvert.SerializeObject(model);
            await _client.PostAsync("https://atron-hmg.azurewebsites.net/api/Usuario/CriarUsuario", json);
            ResultResponses.AddRange(_communicationService.GetResultResponses());
        }
    }
}
