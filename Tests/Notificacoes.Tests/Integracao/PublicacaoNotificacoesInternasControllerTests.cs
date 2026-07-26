using System.Reflection;
using System.Diagnostics.Metrics;
using AtronNotificacoes;
using AtronNotificacoes.Application;
using AtronNotificacoes.Contracts;
using AtronNotificacoes.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Notificacoes.Tests.Integracao;

public sealed class PublicacaoNotificacoesInternasControllerTests
{
    [Fact]
    public void Publicacao_deve_exigir_token_de_servico_e_usar_rota_propria()
    {
        var controllerType = typeof(PublicacaoNotificacoesInternasController);
        var authorize = controllerType.GetCustomAttribute<AuthorizeAttribute>();
        var route = controllerType.GetCustomAttribute<RouteAttribute>();
        var action = controllerType.GetMethod(nameof(PublicacaoNotificacoesInternasController.Publicar));
        var httpPost = action?.GetCustomAttribute<HttpPostAttribute>();

        Assert.Equal(SegurancaNotificacoes.PoliticaPublicador, authorize?.Policy);
        Assert.Equal("api/notificacoes/publicacoes", route?.Template);
        Assert.NotNull(httpPost);
    }

    [Fact]
    public async Task Publicacao_deve_registrar_metrica_quando_o_servico_conclui()
    {
        using var listener = new MeterListener();
        var medidas = new List<string>();
        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument.Meter.Name == ObservabilidadeNotificacoes.NomeDoMedidor)
                meterListener.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<long>((instrument, _, _, _) => medidas.Add(instrument.Name));
        listener.Start();

        var controller = new PublicacaoNotificacoesInternasController(
            new NotificacaoInternaServiceDeTeste(),
            NullLogger<PublicacaoNotificacoesInternasController>.Instance);

        await controller.Publicar(CriarRequest());

        Assert.Contains(ObservabilidadeNotificacoes.NomeContadorPublicacoes, medidas);
    }

    [Fact]
    public async Task Publicacao_deve_registrar_metrica_quando_o_servico_falha()
    {
        using var listener = new MeterListener();
        var medidas = new List<string>();
        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument.Meter.Name == ObservabilidadeNotificacoes.NomeDoMedidor)
                meterListener.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<long>((instrument, _, _, _) => medidas.Add(instrument.Name));
        listener.Start();

        var controller = new PublicacaoNotificacoesInternasController(
            new NotificacaoInternaServiceComFalha(),
            NullLogger<PublicacaoNotificacoesInternasController>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() => controller.Publicar(CriarRequest()));

        Assert.Contains(ObservabilidadeNotificacoes.NomeContadorFalhasDePublicacao, medidas);
    }

    private static PublicarNotificacaoInternaRequest CriarRequest() =>
        new("USR001", "Tracker", "TarefaAtribuida", "Título", "Mensagem", null, "tarefa:1", DateTimeOffset.UtcNow, "teste:1", "correlacao-teste");

    private sealed class NotificacaoInternaServiceDeTeste : INotificacaoInternaService
    {
        public Task<NotificacaoInternaResponse> CriarAsync(PublicarNotificacaoInternaRequest request) =>
            Task.FromResult(new NotificacaoInternaResponse(1000001, request.ModuloOrigem, request.TipoEvento, request.Titulo,
                request.Mensagem, request.UrlDestino, request.ReferenciaExterna, false, request.DataCriacao, null));

        public Task<IReadOnlyList<NotificacaoInternaResponse>> ObterMinhasAsync(string destinatarioCodigo) => throw new NotSupportedException();
        public Task<NotificacaoInternaResponse?> MarcarComoLidaAsync(long id, string destinatarioCodigo) => throw new NotSupportedException();
        public Task<IReadOnlyList<NotificacaoInternaResponse>> MarcarTodasComoLidasAsync(string destinatarioCodigo) => throw new NotSupportedException();
        public Task<bool> ExcluirAsync(long id, string destinatarioCodigo) => throw new NotSupportedException();
    }

    private sealed class NotificacaoInternaServiceComFalha : INotificacaoInternaService
    {
        public Task<NotificacaoInternaResponse> CriarAsync(PublicarNotificacaoInternaRequest request) =>
            throw new InvalidOperationException("Falha simulada.");

        public Task<IReadOnlyList<NotificacaoInternaResponse>> ObterMinhasAsync(string destinatarioCodigo) => throw new NotSupportedException();
        public Task<NotificacaoInternaResponse?> MarcarComoLidaAsync(long id, string destinatarioCodigo) => throw new NotSupportedException();
        public Task<IReadOnlyList<NotificacaoInternaResponse>> MarcarTodasComoLidasAsync(string destinatarioCodigo) => throw new NotSupportedException();
        public Task<bool> ExcluirAsync(long id, string destinatarioCodigo) => throw new NotSupportedException();
    }
}
