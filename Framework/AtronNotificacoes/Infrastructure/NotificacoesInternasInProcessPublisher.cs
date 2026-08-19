using System.Transactions;
using AtronNotificacoes.Application.Interfaces;
using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Contracts.DTO.Response;
using AtronNotificacoes.Contracts.Interfaces;
using AtronNotificacoes.Observability;
using AtronNotificacoes.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AtronNotificacoes.Infrastructure;

public sealed class NotificacoesInternasInProcessPublisher(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<NotificacoesInternasInProcessPublisher> logger) : INotificacoesInternasPublisher
{
    public async Task<ResultadoPublicacaoNotificacaoInterna> PublicarAsync(
        PublicarNotificacaoInternaRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var transacaoSuprimida = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);

            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<INotificacaoInternaService>();
            var notificacao = await service.CriarAsync(request);

            transacaoSuprimida.Complete();

            ObservabilidadeNotificacoes.RegistrarPublicacao(request);

            logger.LogInformation(NotificacoesResource.Log_Publicacao, request.ModuloOrigem, request.TipoEvento, request.CorrelacaoId);

            return ResultadoPublicacaoNotificacaoInterna.Sucesso(notificacao);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            ObservabilidadeNotificacoes.RegistrarFalhaDePublicacao(request);

            logger.LogError(exception, NotificacoesResource.Log_FalhaPublicacao, request.ModuloOrigem, request.TipoEvento, request.CorrelacaoId);

            return ResultadoPublicacaoNotificacaoInterna.Falha(
                NotificacoesResource.Erro_Publicacao);
        }
    }
}
