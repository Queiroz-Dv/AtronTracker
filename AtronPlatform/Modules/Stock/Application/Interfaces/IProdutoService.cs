using AtronStock.Application.DTO.Request;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<Resultado> RegistrarProdutoAsync(ProdutoRequest produto);
        //Task RegistrarProdutosEmLoteAsync(Produto produtoBase, int quantidade);
        //Task InativarProdutoAsync(int produtoId);
        //Task InativarProdutosEmLoteAsync(List<int> produtoIds);

        //Task<Resultado<ICollection<Produto>>> ObterTodos();
    }
}
