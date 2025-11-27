using AtronStock.Domain.Entities;

namespace AtronStock.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task<bool> CriarClienteAsync(Cliente cliente);

        Task<ICollection<Cliente>> ObterTodoClientesAsync();

        Task<ICollection<Cliente>> ObterTodoClientesInativosAsync();

        Task<Cliente> ObterClientePorCodigoAsync(string codigo);

        Task<bool> AtualizarClienteAsync(Cliente cliente);        
    }
}