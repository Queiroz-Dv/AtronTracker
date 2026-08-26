using AtronStock.Domain.Entities;

namespace AtronStock.Domain.Interfaces;

public interface ILoteProdutoRepository
{
    Task<IReadOnlyCollection<string>> ObterCodigosPorPrefixoAsync(string prefixo);
    Task<IReadOnlyCollection<string>> ObterCodigosProdutosExistentesAsync(
        IReadOnlyCollection<string> codigos);
    Task<bool> AdicionarAsync(LoteProduto lote);
}
