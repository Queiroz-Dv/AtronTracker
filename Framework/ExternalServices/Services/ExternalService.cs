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

        public async Task AtualizarPorId(string id, DTO dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            await _apiClient.PutAsyncById(Convert.ToInt32(id), json);
        }

        public async Task Criar(DTO dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            await _apiClient.PostAsync(json);
        }

        public async Task<DTO> ObterPorCodigo(string codigo)
        {
            var response = await _apiClient.GetAsync(codigo);
            return JsonConvert.DeserializeObject<DTO>(response);
        }

        public async Task<DTO> ObterPorId(string id)
        {
            var response = await _apiClient.GetAsync(Convert.ToInt32(id));
            return JsonConvert.DeserializeObject<DTO>(response);
        }

        public async Task<List<DTO>> ObterTodos()
        {
            var entities = await _apiClient.GetAsync();
            return JsonConvert.DeserializeObject<List<DTO>>(entities);            
        }

        public async Task<List<T>> ObterTodos<T>()
        {
            var entities = await _apiClient.GetAsync();
            return JsonConvert.DeserializeObject<List<T>>(entities);
        }

        public async Task Remover(string codigo)
        {
            await _apiClient.DeleteAsync(codigo);
        }
    }
}