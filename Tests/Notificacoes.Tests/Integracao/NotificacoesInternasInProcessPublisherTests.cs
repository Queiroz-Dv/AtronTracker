using System.Diagnostics.Metrics;
using System.Transactions;
using AtronNotificacoes.Application.Interfaces;
using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Contracts.DTO.Response;
using AtronNotificacoes.Contracts.Interfaces;
using AtronNotificacoes.Infrastructure;
using AtronNotificacoes.Observability;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Notificacoes.Tests.Integracao;

public sealed class NotificacoesInternasInProcessPublisherTests
{
    [Fact]
    public async Task PublicarAsync_deve_publicar_em_escopo_proprio_sem_transacao_ambiente()
    {
        using var provider = CriarProvider<NotificacaoInternaServiceDeTeste>();
        var publisher = provider.GetRequiredService<INotificacoesInternasPublisher>();
        using var transacaoAmbiente = new TransactionScope(
            TransactionScopeAsyncFlowOption.Enabled);

        var resultado = await publisher.PublicarAsync(CriarRequest());
        var service = provider.GetRequiredService<NotificacaoInternaServiceDeTeste>();

        Assert.True(resultado.Publicada);
        Assert.False(service.PossuiaTransacaoAmbiente);
    }

    [Fact]
    public async Task PublicarAsync_deve_retornar_falha_consultiva_e_registrar_metrica()
    {
        using var listener = new MeterListener();
        var medidas = new List<string>();
        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument.Meter.Name == ObservabilidadeNotificacoes.NomeDoMedidor)
                meterListener.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<long>((instrument, _, _, _) =>
            medidas.Add(instrument.Name));
        listener.Start();

        using var provider = CriarProvider<NotificacaoInternaServiceComFalha>();
        var publisher = provider.GetRequiredService<INotificacoesInternasPublisher>();

        var resultado = await publisher.PublicarAsync(CriarRequest());

        Assert.False(resultado.Publicada);
        Assert.Contains(
            ObservabilidadeNotificacoes.NomeContadorFalhasDePublicacao,
            medidas);
    }

    private static ServiceProvider CriarProvider<TService>()
        where TService : class, INotificacaoInternaService
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<TService>();
        services.AddScoped<INotificacaoInternaService>(provider =>
            provider.GetRequiredService<TService>());
        services.AddScoped<INotificacoesInternasPublisher, NotificacoesInternasInProcessPublisher>();
        return services.BuildServiceProvider();
    }

    private static PublicarNotificacaoInternaRequest CriarRequest() =>
        new()
        {
            DestinatarioCodigo = "USR001",
            ModuloOrigem = "Tracker",
            TipoEvento = "TarefaCriada",
            Titulo = "Nova tarefa",
            Mensagem = "Uma tarefa foi criada.",
            UrlDestino = null,
            ReferenciaExterna = "tarefa:123",
            DataCriacao = DateTimeOffset.Parse("2026-07-19T12:00:00Z"),
            ChaveIdempotencia = "tracker:tarefa:123",
            CorrelacaoId = "correlacao-123"
        };

    private sealed class NotificacaoInternaServiceDeTeste : INotificacaoInternaService
    {
        public bool PossuiaTransacaoAmbiente { get; private set; }

        public Task<NotificacaoInternaResponse> CriarAsync(PublicarNotificacaoInternaRequest request)
        {
            PossuiaTransacaoAmbiente = Transaction.Current is not null;
            return Task.FromResult(new NotificacaoInternaResponse(
                1000001,
                request.ModuloOrigem,
                request.TipoEvento,
                request.Titulo,
                request.Mensagem,
                request.UrlDestino,
                request.ReferenciaExterna,
                false,
                request.DataCriacao,
                null));
        }

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
