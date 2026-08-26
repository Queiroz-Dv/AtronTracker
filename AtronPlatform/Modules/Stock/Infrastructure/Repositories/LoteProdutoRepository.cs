using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AtronStock.Infrastructure.Repositories;

public sealed class LoteProdutoRepository(StockDbContext context) : ILoteProdutoRepository
{
    public async Task<IReadOnlyCollection<string>> ObterCodigosPorPrefixoAsync(string prefixo)
        => await context.LotesProdutos
            .AsNoTracking()
            .Where(lote => lote.Codigo == prefixo || lote.Codigo.StartsWith(prefixo + "_"))
            .Select(lote => lote.Codigo)
            .ToListAsync();

    public async Task<IReadOnlyCollection<string>> ObterCodigosProdutosExistentesAsync(
        IReadOnlyCollection<string> codigos)
        => await context.Produtos
            .AsNoTracking()
            .Where(produto => codigos.Contains(produto.Codigo))
            .Select(produto => produto.Codigo)
            .ToListAsync();

    public async Task<bool> AdicionarAsync(LoteProduto lote)
    {
        await context.LotesProdutos.AddAsync(lote);
        return await context.SaveChangesAsync() > 0;
    }
}
