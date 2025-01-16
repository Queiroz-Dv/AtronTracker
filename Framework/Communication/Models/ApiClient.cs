using Communication.Interfaces;
using Communication.Interfaces.Services;
using Newtonsoft.Json;
using Shared.Models;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Communication.Models
{
    /// <summary>
    /// Classe que implementa todos os processos de comunicação com a API
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ICommunicationService _communicationService;

        /// <summary>
        /// Construtor que realiza a injeção de dependência da classe
        /// </summary>
        /// <param name="httpClient">Classe que é utilizada para fazer a comunicação direta com a API</param>
        /// <param name="communicationService">Interface que irá lidar com as respostas de retorno da API </param>
        public ApiClient(HttpClient httpClient, ICommunicationService communicationService)
        {
            _httpClient = httpClient;
            _communicationService = communicationService;
        }

        public async Task<string> GetAsync(string uri)
        {
            var response = await _httpClient.GetAsync(uri);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        public async Task<string> GetAsync(string uri, string parameter)
        {
            var response = await _httpClient.GetAsync($"{uri}/{parameter}");
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        public async Task PostAsync(string uri, string content)
        {
            var httpContent = new StringContent(content, Encoding.UTF8, Application.Json);
            var response = await _httpClient.PostAsync(uri, httpContent);
            await FillResultResponse(response);
        }

        /// <summary>
        /// Preenche a lista de comunicação com os retornos da resposta da API
        /// </summary>
        /// <param name="response">Mensagem de resposta que será processada</param>       
        private async Task FillResultResponse(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var messages = JsonConvert.DeserializeObject<List<Message>>(responseContent);
            _communicationService.AddMessages(messages);
        }

        public async Task PutAsync(string uri, string parameter, string content)
        {
            var httpContent = new StringContent(content, Encoding.UTF8, Application.Json);
            try
            {
                var response = await _httpClient.PutAsync(uri, httpContent);
                await FillResultResponse(response);
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }

        public async Task DeleteAsync(string uri, string codigo)
        {
            var response = await _httpClient.DeleteAsync(uri);

            await FillResultResponse(response);
        }

        public async Task<DTO> PostAsync<DTO>(string url, string content)
        {
            var httpContent = new StringContent(content, Encoding.UTF8, Application.Json);
            var response = await _httpClient.PostAsync(url, httpContent);

            var responseContent = await response.Content.ReadAsStringAsync();
            
            var jsonContent = JsonConvert.DeserializeObject<DTO>(responseContent);
            return jsonContent;
        }
    }
}