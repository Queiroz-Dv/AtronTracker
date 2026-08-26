#nullable enable

using AtronStock.Application.DTO.Request;
using AtronStock.Application.Resources;
using AtronStock.Application.UseCases.ProdutoCases;
using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Repositories;

namespace AtronStock.Infrastructure.Workers;

public sealed class ProcessadorProdutosLote(
    IProcessamentoProdutoLoteRepository repository,
    ExecutarGeracaoProdutosLoteCase executor,
    ITransactionManager transactionManager,
    IServiceScopeFactory scopeFactory,
    ILogger<ProcessadorProdutosLote> logger)
{
    public async Task ProcessarAsync(
        int processamentoId,
        Guid tokenReserva,
        CancellationToken cancellationToken)
    {
        string? erro = null;
        try
        {
            var processamento = await repository.ObterPorIdAsync(processamentoId);
            if (processamento?.Status != EStatusProcessamentoProdutoLote.EmExecucao
                || processamento.TokenReserva != tokenReserva)
                return;

            using var transaction = transactionManager.CreateScope();
            var resultado = await executor.ExecutarAsync(
                GeracaoProdutosLoteCommand.Criar(processamento));
            if (resultado.TeveFalha)
            {
                erro = string.Join(" ", resultado.Messages.Select(item => item.Descricao));
            }
            else
            {
                processamento.Concluir(
                    resultado.Dados!.LoteProdutoId,
                    resultado.Dados.QuantidadeProcessada,
                    tokenReserva);
                await repository.AtualizarAsync(processamento);
                transaction.Complete();
                return;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Falha ao gerar produtos do processamento {ProcessamentoId}.",
                processamentoId);
            erro = ProdutoResource.ErroProcessamentoProdutosLote;
        }

        await RegistrarFalhaAsync(
            processamentoId,
            tokenReserva,
            erro ?? ProdutoResource.ErroProcessamentoProdutosLote);
    }

    private async Task RegistrarFalhaAsync(
        int processamentoId,
        Guid tokenReserva,
        string erro)
    {
        using var scope = scopeFactory.CreateScope();
        var novoRepository = scope.ServiceProvider
            .GetRequiredService<IProcessamentoProdutoLoteRepository>();
        var processamento = await novoRepository.ObterPorIdAsync(processamentoId);
        if (processamento?.TokenReserva != tokenReserva)
            return;

        processamento.Falhar(erro, tokenReserva);
        await novoRepository.AtualizarAsync(processamento);
    }
}
