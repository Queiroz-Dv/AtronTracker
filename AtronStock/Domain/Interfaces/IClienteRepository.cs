using AtronStock.Domain.Entities;

namespace AtronStock.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task CriarCliente(Cliente cliente);
    }
}
