using Communication.Interfaces;
using Communication.Security;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shared.Models;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Communication.Models
{
    /// <summary>
    /// Classe que implementa todos os processos de comunicação com a API
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly MessageModel _messageModel;
        private readonly HttpClient _httpClient;

        public string Url { get; set; }
        public string Modulo { get; set; }

        public ApiClient(HttpClient httpClient, MessageModel messageModel)
        {
            _httpClient = httpClient;
            _messageModel = messageModel;

            if (!TokenServiceStore.Token.IsNullOrEmpty())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenServiceStore.Token);
            }
        }

        public async Task<string> GetAsync()
        {
            var response = await _httpClient.GetAsync(Url);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        public async Task<string> GetAsync(string parameter)
        {
            var response = await _httpClient.GetAsync($"{Url}{parameter}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            else
            {
                _messageModel.AddError("Não foi possível obter o registro.");
                return null;
            }
        }

        public async Task PostAsync(string content)
        {
            var httpContent = new StringContent(content, Encoding.UTF8, Application.Json);
            var response = await _httpClient.PostAsync(Url, httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            FillMessageModel(responseContent);
        }

        private void FillMessageModel(string responseContent)
        {
            if (responseContent.Contains(nameof(Message)))
            {
                var messages = JsonConvert.DeserializeObject<List<Message>>(responseContent);

                _messageModel.Messages.AddRange(messages);
            }
        }

        public async Task PutAsync(string parameter, string content)
        {
            var httpContent = new StringContent(content, Encoding.UTF8, Application.Json);
            try
            {
                var response = await _httpClient.PutAsync($"{Url}{parameter}", httpContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                FillMessageModel(responseContent);

            }
            catch (HttpRequestException)
            {
                throw;
            }
        }
      
        public async Task DeleteAsync(string codigo)
        {
            var response = await _httpClient.DeleteAsync($"{Url}{codigo}");

            if (response.IsSuccessStatusCode)
            {
                _messageModel.AddRegisterRemovedSuccessMessage(Modulo);
            }
        }

        public async Task<DTO> PostAsync<DTO>(string content)
        {
            var httpContent = new StringContent(content, Encoding.UTF8, Application.Json);
            var response = await _httpClient.PostAsync(Url, httpContent);

            var responseContent = await response.Content.ReadAsStringAsync();
            FillMessageModel(responseContent);

            var jsonContent = JsonConvert.DeserializeObject<DTO>(responseContent);
            return jsonContent;
        }

        public async Task<string> GetAsync(int parameter)
        {
            var response = await _httpClient.GetAsync($"{Url}{parameter}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();                
                FillMessageModel(responseContent);
                return responseContent;
            }
            else
            {
                _messageModel.AddError("Não foi possível obter o registro.");
                return null;
            }
        }

        public async Task PutAsyncById(int parameter, string content)
        {
            var httpContent = new StringContent(content, Encoding.UTF8, Application.Json);
            try
            {
                var response = await _httpClient.PutAsync($"{Url}{parameter}", httpContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                FillMessageModel(responseContent);
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }
    }
}