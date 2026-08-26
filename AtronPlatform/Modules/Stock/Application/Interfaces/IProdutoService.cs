using AtronStock.Application.DTO.Request;
using AtronStock.Application.DTO.Response;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<Resultado> CriarAsync(ProdutoRequest request);
        Task<Resultado> AtualizarAsync(string codigo, ProdutoAtualizacaoRequest request);
        Task<Resultado<ICollection<ProdutoResponse>>> ObterTodosAsync();
        Task<Resultado<ProdutoResponse>> ObterPorCodigoAsync(string codigo);
        Task<Resultado<SolicitacaoGeracaoProdutosLoteResponse>> SolicitarGeracaoLoteAsync(GerarProdutosLoteRequest request);
    }
}
