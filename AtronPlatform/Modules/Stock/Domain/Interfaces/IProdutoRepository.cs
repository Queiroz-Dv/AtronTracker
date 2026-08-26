using AtronStock.Domain.Entities;

namespace AtronStock.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto?> ObterPorIdAsync(int id);
        Task<Produto?> ObterPorCodigoAsync(string codigo);
        Task<ICollection<Produto>> ObterTodosAsync();
        Task<bool> AdicionarAsync(Produto produto);
        Task<bool> AtualizarAsync(Produto produto);
    }
}
