using AtronStock.Application.DTO.Request;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.Interfaces
{
    public interface IClienteService
    {
        Task<Resultado> CriarAsync(ClienteRequest request);
        
        Task<Resultado> AtualizarClienteServiceAsync(ClienteRequest request);
        
        Task<Resultado> RemoverAsync(string codigo);

        Task<Resultado<ICollection<ClienteRequest>>> ObterTodosClientesServiceAsync();

        Task<Resultado<ClienteRequest>> ObterClientePorCodigoServiceAsync(string codigo);
    }
}