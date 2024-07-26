using Atron.Domain.Entities;
using Atron.Domain.ViewsInterfaces;
using Newtonsoft.Json;
using Notification.Models;
using Shared.DTO;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Atron.Infrastructure.ViewsRepositories
{
    public class DepartamentoViewRepository : IDepartamentoViewRepository
    {
        public List<NotificationMessage> _messages { get; }

        public List<ResultResponse> ResultApiJson { get; }

        private readonly IHttpClientFactory _httpClientFactory;

        public DepartamentoViewRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _messages = new List<NotificationMessage>();
            ResultApiJson = new List<ResultResponse>();
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

        public async Task CriarDepartamento(Departamento departamento)
        {
            var json = JsonConvert.SerializeObject(departamento, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, Application.Json);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://atron-hmg.azurewebsites.net/api/Departamento/CriarDepartamento"),
                Method = HttpMethod.Post,
                Content = content
            };

            var client = await _httpClientFactory.CreateClient().SendAsync(request);
            var responseContent = await client.Content.ReadAsStringAsync();

            if (client.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<ResultResponse>>(responseContent);
                ResultApiJson.AddRange(result);
            }
            else
            {
                var errorResponse = await client.ReadValidationErrorResponseAsync();

                if (errorResponse?.Errors != null)
                {
                    foreach (var error in errorResponse.Errors)
                    {
                        foreach (var message in error.Value)
                        {
                            _messages.Add(new NotificationMessage(message, "Error"));
                        }
                    }
                }
            }
            return;
        }
    }
}