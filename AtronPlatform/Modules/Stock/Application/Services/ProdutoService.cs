using AtronStock.Application.DTO.Request;
using AtronStock.Application.DTO.Response;
using AtronStock.Application.Interfaces;
using AtronStock.Application.UseCases.ProdutoCases;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.Services
{
    public sealed class ProdutoService(
        CriarProdutoCase criarProduto,
        AtualizarProdutoCase atualizarProduto,
        ObterProdutoCase obterProduto,
        SolicitarGeracaoProdutosLoteCase solicitarGeracaoLote) : IProdutoService
    {
        private readonly CriarProdutoCase _criarProduto = criarProduto;
        private readonly AtualizarProdutoCase _atualizarProduto = atualizarProduto;
        private readonly ObterProdutoCase _obterProduto = obterProduto;
        private readonly SolicitarGeracaoProdutosLoteCase _solicitarGeracaoLote
            = solicitarGeracaoLote;

        public Task<Resultado> CriarAsync(ProdutoRequest request)
            => _criarProduto.ExecutarAsync(request);

        public Task<Resultado> AtualizarAsync(
            string codigo,
            ProdutoAtualizacaoRequest request)
            => _atualizarProduto.ExecutarAsync(codigo, request);

        public Task<Resultado<ICollection<ProdutoResponse>>> ObterTodosAsync()
            => _obterProduto.ObterTodosAsync();

        public Task<Resultado<ProdutoResponse>> ObterPorCodigoAsync(string codigo)
            => _obterProduto.ObterPorCodigoAsync(codigo);

        public Task<Resultado<SolicitacaoGeracaoProdutosLoteResponse>> SolicitarGeracaoLoteAsync(
            GerarProdutosLoteRequest request)
            => _solicitarGeracaoLote.ExecutarAsync(request);
    }
}
