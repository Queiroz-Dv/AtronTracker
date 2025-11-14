using AtronStock.Application.DTO.Request;
using Shared.Models;

namespace AtronStock.Application.Interfaces
{
    public interface IClienteService
    {
        Task<Resultado> CriarAsync(ClienteRequest request);

        // Assinatura de atualização adicionada
        Task<Resultado> AtualizarClienteServiceAsync(ClienteRequest request);

        // Assinatura de remoção adicionada
        Task<Resultado> RemoverAsync(string codigo);

        Task<Resultado<ICollection<ClienteRequest>>> ObterTodosClientesServiceAsync();

        Task<Resultado<ClienteRequest>> ObterClientePorCodigoServiceAsync(string codigo);
    }
}