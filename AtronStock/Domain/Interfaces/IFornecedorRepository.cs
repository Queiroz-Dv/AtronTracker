using AtronStock.Domain.Entities;

namespace AtronStock.Domain.Interfaces
{
    public interface IFornecedorRepository
    {
        Task<Fornecedor?> ObterPorIdAsync(int id);
        Task<bool> AdicionarAsync(Fornecedor fornecedor);
        Task<ICollection<Fornecedor>> ListarTodosAsync();
        Task<Fornecedor> ObterPorCodigoAsync(string codigo);
    }
}
