using Communication.Interfaces;
using ExternalServices.Interfaces;
using Newtonsoft.Json;

namespace ExternalServices.Services
{
    public class ExternalService<DTO> : IExternalService<DTO>
    {
        private readonly IApiClient _apiClient;

        public ExternalService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task Atualizar(string codigo, DTO dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            await _apiClient.PutAsync(codigo, json);
        }
        public async Task Criar(DTO dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            await _apiClient.PostAsync(json);
        }

        public async Task<DTO> ObterPorCodigo(string codigo)
        {
            var response = await _apiClient.GetAsync(codigo);
            var dto = JsonConvert.DeserializeObject<DTO>(response);
            return dto;
        }

        public async Task<List<DTO>> ObterTodos()
        {
            var entities = await _apiClient.GetAsync();
            var dtos = JsonConvert.DeserializeObject<List<DTO>>(entities);
            return dtos is not null ? dtos : new List<DTO>();
        }

        public async Task Remover(string codigo)
        {
            await _apiClient.DeleteAsync(codigo);
        }
    }
}