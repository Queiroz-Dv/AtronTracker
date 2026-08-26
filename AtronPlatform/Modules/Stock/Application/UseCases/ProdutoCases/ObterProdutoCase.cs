using AtronStock.Application.DTO.Response;
using AtronStock.Application.Mapping;
using AtronStock.Application.Resources;
using AtronStock.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.UseCases.ProdutoCases
{
    public sealed class ObterProdutoCase(IProdutoRepository repository, ProdutoMapping mapper)
    {
        private readonly IProdutoRepository _repository = repository;
        private readonly ProdutoMapping _mapper = mapper;

        public async Task<Resultado<ICollection<ProdutoResponse>>> ObterTodosAsync()
        {
            var produtos = await _repository.ObterTodosAsync();

            return Resultado<ICollection<ProdutoResponse>>.Sucesso(produtos.Select(_mapper.MapToDto).ToList());
        }

        public async Task<Resultado<ProdutoResponse>> ObterPorCodigoAsync(string codigo)
        {
            var produto = await _repository.ObterPorCodigoAsync(codigo.NormalizarCodigo());
            return produto is null
                ? Resultado<ProdutoResponse>.Falha(string.Format(ProdutoResource.ErroProdutoNaoEncontrado, codigo))
                : Resultado<ProdutoResponse>.Sucesso(_mapper.MapToDto(produto));
        }
    }
}