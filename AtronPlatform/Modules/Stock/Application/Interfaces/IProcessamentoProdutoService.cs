using AtronStock.Application.DTO.Response;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.Interfaces;

public interface IProcessamentoProdutoService
{
    Task<Resultado<ICollection<ProcessamentoProdutoResponse>>> ObterMeusAsync();
    Task<Resultado<ProcessamentoProdutoResponse>> ObterPorIdAsync(int id);
}
