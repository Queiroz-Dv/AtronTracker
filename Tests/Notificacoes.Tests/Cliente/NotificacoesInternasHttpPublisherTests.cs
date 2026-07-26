using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using AtronNotificacoes.Client;
using AtronNotificacoes.Contracts;
using Xunit;

namespace Notificacoes.Tests.Cliente;

public sealed class NotificacoesInternasHttpPublisherTests
{
    [Fact]
    public async Task PublicarAsync_envia_token_de_servico_correlacao_e_payload()
    {
        var handler = new StubHttpMessageHandler(HttpStatusCode.Created, new NotificacaoInternaResponse(
            1000001, "Tracker", "TarefaCriada", "Nova tarefa", "Uma tarefa foi criada.", null, null, false,
            DateTimeOffset.Parse("2026-07-19T12:00:00Z"), null));
        var publisher = CriarPublisher(handler);
        var request = CriarRequest("correlacao-123");

        var resultado = await publisher.PublicarAsync(request);

        Assert.True(resultado.Publicada);
        Assert.Equal(1000001, resultado.Notificacao?.Id);
        Assert.Equal("/api/notificacoes/publicacoes", handler.Request?.RequestUri?.AbsolutePath);
        Assert.Equal("correlacao-123", handler.Request?.Headers.GetValues("X-Correlation-Id").Single());
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(handler.Request?.Headers.Authorization?.Parameter);
        Assert.Equal("atron-notificacoes", jwt.Audiences.Single());
        Assert.Contains(jwt.Claims, claim => claim.Type == "tipo_token" && claim.Value == "servico");
        Assert.Contains(jwt.Claims, claim => claim.Type == "escopo" && claim.Value == "notificacoes.publicar");
    }

    [Fact]
    public async Task PublicarAsync_retorna_falha_consultiva_quando_o_servico_recusa_a_publicacao()
    {
        var publisher = CriarPublisher(new StubHttpMessageHandler(HttpStatusCode.ServiceUnavailable, null));

        var resultado = await publisher.PublicarAsync(CriarRequest(null));

        Assert.False(resultado.Publicada);
        Assert.Null(resultado.Notificacao);
        Assert.Equal("O serviço de notificações recusou a publicação.", resultado.MotivoDaFalha);
    }

    [Fact]
    public async Task Consulta_repassa_o_token_do_usuario_para_a_rota_da_central()
    {
        var handler = new ConsultaStubHttpMessageHandler();
        var consulta = new NotificacoesInternasHttpConsultaClient(
            new HttpClient(handler),
            new Uri("https://notificacoes.teste/"));

        var resultado = await consulta.ObterMinhasAsync("token-do-usuario");

        Assert.True(resultado.Sucesso);
        Assert.Single(resultado.Dados!);
        Assert.Equal("/api/notificacoes", handler.Request?.RequestUri?.AbsolutePath);
        Assert.Equal("Bearer", handler.Request?.Headers.Authorization?.Scheme);
        Assert.Equal("token-do-usuario", handler.Request?.Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task ExcluirAsync_repassa_o_token_do_usuario_para_a_rota_da_central()
    {
        var handler = new ConsultaStubHttpMessageHandler();
        var consulta = new NotificacoesInternasHttpConsultaClient(
            new HttpClient(handler),
            new Uri("https://notificacoes.teste/"));

        var excluida = await consulta.ExcluirAsync(1000001, "token-do-usuario");

        Assert.True(excluida);
        Assert.Equal(HttpMethod.Delete, handler.Request?.Method);
        Assert.Equal("/api/notificacoes/1000001", handler.Request?.RequestUri?.AbsolutePath);
        Assert.Equal("token-do-usuario", handler.Request?.Headers.Authorization?.Parameter);
    }

    private static NotificacoesInternasHttpPublisher CriarPublisher(HttpMessageHandler handler) =>
        new(new HttpClient(handler), new NotificacoesInternasServiceOptions(
            new Uri("https://notificacoes.teste/"),
            "atron-notificacoes-service",
            "atron-notificacoes",
            "segredo-de-servico-para-teste-com-tamanho-seguro",
            "tracker"));

    private static PublicarNotificacaoInternaRequest CriarRequest(string? correlacaoId) =>
        new("USR001", "Tracker", "TarefaCriada", "Nova tarefa", "Uma tarefa foi criada.", null, "tarefa:123",
            DateTimeOffset.Parse("2026-07-19T12:00:00Z"), "tracker:tarefa:123", correlacaoId);

    private sealed class StubHttpMessageHandler(HttpStatusCode statusCode, NotificacaoInternaResponse? resposta) : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            var response = new HttpResponseMessage(statusCode);
            if (resposta is not null)
                response.Content = JsonContent.Create(resposta);

            await Task.CompletedTask;
            return response;
        }
    }

    private sealed class ConsultaStubHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create<IReadOnlyList<NotificacaoInternaResponse>>(
                [
                    new NotificacaoInternaResponse(
                        1000001, "Tracker", "TarefaCriada", "Nova tarefa", "Mensagem", null, null, false,
                        DateTimeOffset.Parse("2026-07-19T12:00:00Z"), null)
                ])
            };

            await Task.CompletedTask;
            return response;
        }
    }
}
