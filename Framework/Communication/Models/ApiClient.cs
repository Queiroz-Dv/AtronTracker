using Communication.Interfaces;
using Communication.Interfaces.Services;
using Newtonsoft.Json;
using Shared.DTO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Communication.Models
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ICommunicationService _communicationService;

        public ApiClient(HttpClient httpClient, ICommunicationService communicationService)
        {
            _httpClient = httpClient;
            _communicationService = communicationService;
        }

        public async Task<string> GetAsync(string uri)
        {
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        public async Task PostAsync(string uri, string content)
        {
            var httpContent = new StringContent(content, Encoding.UTF8, Application.Json);
            var response = await _httpClient.PostAsync(uri, httpContent);
            response.EnsureSuccessStatusCode();
            await FillResultResponse(response);
        }

        private async Task FillResultResponse(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var resultContent = JsonConvert.DeserializeObject<List<ResultResponse>>(responseContent);
            _communicationService.AddResponseContent(resultContent);
        }

        public async Task PutAsync(string uri, string parameter, string content)
        {
            var uriFormated = $"{uri}{parameter}";
            var httpContent = new StringContent(content, Encoding.UTF8, Application.Json);
            try
            {
                var response = await _httpClient.PutAsync(uriFormated, httpContent);
                await FillResultResponse(response);
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }

        public async Task DeleteAsync(string uri, string codigo)
        {
            var uriFormated = $"{uri}{codigo}";

            var response = await _httpClient.DeleteAsync(uriFormated);

            await FillResultResponse(response);
        }
    }
}
