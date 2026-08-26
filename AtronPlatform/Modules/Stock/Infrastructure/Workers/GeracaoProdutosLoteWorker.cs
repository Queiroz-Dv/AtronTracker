#nullable enable

using AtronStock.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AtronStock.Infrastructure.Workers;

public sealed class GeracaoProdutosLoteWorker(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    ILogger<GeracaoProdutosLoteWorker> logger) : BackgroundService
{
    private static readonly TimeSpan IntervaloSemTrabalho = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan DuracaoReserva = TimeSpan.FromMinutes(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processamentoId = await ReservarProximoAsync();
                if (processamentoId is null)
                {
                    await Task.Delay(IntervaloSemTrabalho, stoppingToken);
                    continue;
                }

                using var scope = scopeFactory.CreateScope();
                var processador = scope.ServiceProvider
                    .GetRequiredService<ProcessadorProdutosLote>();
                await processador.ProcessarAsync(
                    processamentoId.Id,
                    processamentoId.TokenReserva,
                    stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Falha no ciclo do worker de produtos em lote.");
                await Task.Delay(IntervaloSemTrabalho, stoppingToken);
            }
        }
    }

    private async Task<ReservaProcessamento?> ReservarProximoAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider
            .GetRequiredService<IProcessamentoProdutoLoteRepository>();
        var processamento = await repository.ReservarProximoDisponivelAsync(
            timeProvider.GetUtcNow(),
            DuracaoReserva);
        return processamento?.TokenReserva is Guid token
            ? new ReservaProcessamento(processamento.Id, token)
            : null;
    }

    private sealed record ReservaProcessamento(int Id, Guid TokenReserva);
}
