using Atron.Domain.Entities;
using Atron.Domain.ViewsInterfaces;
using Newtonsoft.Json;
using Notification.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Atron.Infrastructure.ViewsRepositories
{
    public class DepartamentoViewRepository : IDepartamentoViewRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;


        public DepartamentoViewRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            Messages = new List<NotificationMessage>();
        }

        public List<NotificationMessage> Messages { get; }

        public async Task CriarDepartamento(Departamento departamento)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://atron-hmg.azurewebsites.net/api/Departamento/CriarDepartamento");
            request.Method = HttpMethod.Post;

            var json = JsonConvert.SerializeObject(departamento);
            var mediaType = MediaTypeNames.Application.Json;

            var content = new StringContent(json, Encoding.UTF8, mediaType);

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(request.RequestUri, content);
            
            var responseContent = await response.Content.ReadAsStringAsync();

            var dictJson = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseContent);

            
            foreach (var dict in dictJson) {

                var message = dict.ContainsKey("message") ? dict["message"].ToString() : null;
                var notification = new NotificationMessage(message);
                Messages.Add(notification);           
            }

            return;
        }

        public async Task<List<Dictionary<string, object>>> GetDepartamentosAsync()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://atron-hmg.azurewebsites.net/api/Departamento/ObterDepartamentos"),
                Headers = { { "Accept", "application/json" } }
            };

            var client = _httpClientFactory.CreateClient();
            var httpResponse = await client.SendAsync(request);

            if (httpResponse.IsSuccessStatusCode)
            {
                var content = await httpResponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content);
            }
            else
            {
                return null;
            }
        }
    }
}