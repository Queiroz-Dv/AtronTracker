using Microsoft.Extensions.Options;
using Shared.Application.DTOS.Email;
using Shared.Application.DTOS.Requests;
using Shared.Application.Services.Email;
using Shared.Application.Validacoes;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Shared.Tests.Email;

public class SharedEmailServiceTests
{
    [Fact]
    public async Task EnviarAsyncDeveEncaminharHtmlParaBrevo()
    {
        var handler = new CapturingHandler();
        var service = new SharedEmailService(
            Options.Create(new EmailSettings
            {
                Provider = "Brevo",
                FromName = "Atron",
                FromEmail = "sistema@atron.com",
                Brevo = new BrevoEmailSettings
                {
                    ApiKey = "chave-de-teste",
                    BaseUrl = "https://brevo.teste"
                }
            }),
            new EmailValidador(),
            new HttpClientFactoryFake(handler));

        var resultado = await service.EnviarAsync(new EmailRequest
        {
            Assunto = "Assunto",
            Mensagem = "<h1>HTML</h1>",
            EmailsDestino = ["pessoa@exemplo.com"]
        });

        Assert.False(resultado.TeveFalha);
        Assert.Equal(HttpMethod.Post, handler.Method);
        Assert.Equal("https://brevo.teste/smtp/email", handler.Uri);
        using var payload = JsonDocument.Parse(handler.Body);
        Assert.Equal("<h1>HTML</h1>", payload.RootElement.GetProperty("htmlContent").GetString());
    }

    private sealed class HttpClientFactoryFake(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler, disposeHandler: false);
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        public HttpMethod? Method { get; private set; }

        public string? Uri { get; private set; }

        public string Body { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Method = request.Method;
            Uri = request.RequestUri?.ToString();
            Body = request.Content is null ? string.Empty : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
        }
    }
}
