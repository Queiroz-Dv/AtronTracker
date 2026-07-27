using AtronStock.Application.DTO.Request;
using AtronStock.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.Interfaces
{
    public interface IFornecedorService
    {
        Task<Resultado> RegistrarFornecedorAsync(FornecedorRequest fornecedor);

        Task<ICollection<Fornecedor>> ListarFornecedoresAsync();

        Task<Resultado<FornecedorRequest>> ObterFornecedorPorCodigoAsync(string codigo);
    }
}
