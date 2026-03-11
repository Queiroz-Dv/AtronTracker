using AtronStock.Domain.Entities;

namespace AtronStock.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto?> ObterPorIdAsync(int id);
        Task<bool> AdicionarAsync(Produto produto);
        Task AtualizarAsync(Produto produto);

        Task<ICollection<Produto>> ObterTodos();
        Task<Produto> ObterPorCodigoAsync(string codigo);
    }
}
