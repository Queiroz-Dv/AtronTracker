using Communication.Interfaces;
using ExternalServices.Interfaces;
using Newtonsoft.Json;
using Shared.Extensions;
using Shared.Models;

namespace ExternalServices.Services
{
    public class ExternalService<DTO> : IExternalService<DTO>
    {
        private readonly IApiClient _apiClient;
        private readonly MessageModel _messageModel;

        public ExternalService(IApiClient apiClient, MessageModel messageModel)
        {
            _apiClient = apiClient;
            _messageModel = messageModel;
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
            DTO? dto = default;

            if (!_messageModel.Messages.HasErrors())
            {
                dto = JsonConvert.DeserializeObject<DTO>(response);
            }

            return dto;
        }

        public async Task<DTO> ObterPorId(string id)
        {
            var response = await _apiClient.GetAsync(Convert.ToInt32(id));
            DTO? dto = default;

            if (!_messageModel.Messages.HasErrors())
            {
                dto = JsonConvert.DeserializeObject<DTO>(response);
            }

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