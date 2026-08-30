#nullable enable

using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using System.Data;

namespace AtronStock.Infrastructure.Repositories;

public sealed class ProcessamentoProdutoLoteRepository(StockDbContext context)
    : IProcessamentoProdutoLoteRepository
{
    public async Task<bool> AdicionarAsync(ProcessamentoProdutoLote processamento)
    {
        await context.ProcessamentosProdutosLote.AddAsync(processamento);
        return await context.SaveChangesAsync() > 0;
    }

    public Task<ProcessamentoProdutoLote?> ObterPorIdAsync(int id)
        => context.ProcessamentosProdutosLote.FirstOrDefaultAsync(item => item.Id == id);

    public async Task<ProcessamentoProdutoLote?> ReservarProximoDisponivelAsync(
        DateTimeOffset agora,
        TimeSpan duracaoReserva)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.ReadCommitted);
        var pendente = EStatusProcessamentoProdutoLote.Pendente.GetDescription();
        var emExecucao = EStatusProcessamentoProdutoLote.EmExecucao.GetDescription();
        var processamento = await context.ProcessamentosProdutosLote
            .FromSqlInterpolated($$"""
                SELECT * FROM "ProcessamentosProdutosLote"
                WHERE "Status" = {{pendente}}
                   OR ("Status" = {{emExecucao}} AND "ReservaExpiraEm" <= {{agora}})
                ORDER BY "Id"
                FOR UPDATE SKIP LOCKED
                LIMIT 1
                """)
            .FirstOrDefaultAsync();
        if (processamento is null)
        {
            await transaction.CommitAsync();
            return null;
        }

        processamento.Reservar(agora, duracaoReserva, Guid.NewGuid());
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return processamento;
    }

    public async Task<bool> AtualizarAsync(ProcessamentoProdutoLote processamento)
        => await context.SaveChangesAsync() > 0;

    public async Task<ICollection<ProcessamentoProdutoLote>> ObterMeusAsync(
        string solicitanteCodigo)
        => await ConsultaDoSolicitante(solicitanteCodigo)
            .OrderByDescending(item => item.Id)
            .ToListAsync();

    public Task<ProcessamentoProdutoLote?> ObterPorIdDoSolicitanteAsync(
        int id,
        string solicitanteCodigo)
        => ConsultaDoSolicitante(solicitanteCodigo)
            .FirstOrDefaultAsync(item => item.Id == id);

    private IQueryable<ProcessamentoProdutoLote> ConsultaDoSolicitante(
        string solicitanteCodigo)
        => context.ProcessamentosProdutosLote
            .AsNoTracking()
            .Include(item => item.LoteProduto)
            .Where(item => item.Solicitacao.SolicitanteCodigo == solicitanteCodigo);
}
