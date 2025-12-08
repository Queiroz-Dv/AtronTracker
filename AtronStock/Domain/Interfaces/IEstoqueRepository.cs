using AtronStock.Domain.Entities;

namespace AtronStock.Domain.Interfaces
{
    public interface IEstoqueRepository
    {
        Task<Estoque?> ObterPorProdutoIdAsync(int produtoId);
        Task AdicionarAsync(Estoque estoque);
        Task AtualizarAsync(Estoque estoque);
        Task AdicionarMovimentacaoAsync(MovimentacaoEstoque movimentacao);

        // Novos métodos para fluxo complexo
        Task RegistrarEntradaCompletaAsync(Entrada entrada);
        Task RegistrarVendaCompletaAsync(Venda venda);
    }
}
