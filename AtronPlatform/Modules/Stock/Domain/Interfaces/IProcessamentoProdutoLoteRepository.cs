#nullable enable

using AtronStock.Domain.Entities;

namespace AtronStock.Domain.Interfaces;

public interface IProcessamentoProdutoLoteRepository
{
    Task<bool> AdicionarAsync(ProcessamentoProdutoLote processamento);
    Task<ProcessamentoProdutoLote?> ObterPorIdAsync(int id);
    Task<ProcessamentoProdutoLote?> ReservarProximoDisponivelAsync(
        DateTimeOffset agora,
        TimeSpan duracaoReserva);
    Task<bool> AtualizarAsync(ProcessamentoProdutoLote processamento);
    Task<ICollection<ProcessamentoProdutoLote>> ObterMeusAsync(string solicitanteCodigo);
    Task<ProcessamentoProdutoLote?> ObterPorIdDoSolicitanteAsync(
        int id,
        string solicitanteCodigo);
}
