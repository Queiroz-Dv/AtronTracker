using AtronStock.Application.Resources;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.UseCases.ProdutoCases
{
    public sealed class SelecionarCategoriasProdutoCase(ICategoriaRepository repository)
    {
        private readonly ICategoriaRepository _repository = repository;

        public async Task<Resultado<IReadOnlyCollection<Categoria>>> ExecutarAsync(
            IEnumerable<string>? codigosInformados)
        {
            var codigos = (codigosInformados ?? [])
                .Where(codigo => !string.IsNullOrWhiteSpace(codigo))
                .Select(codigo => codigo.Trim().ToUpperInvariant())
                .Distinct()
                .ToArray();

            if (codigos.Length == 0)
                return Resultado<IReadOnlyCollection<Categoria>>.Sucesso([]);

            var categorias = await _repository.ObterPorCodigosAsync(codigos);
            if (categorias.Count != codigos.Length ||
                categorias.Any(categoria => categoria.Status != EStatus.Ativo))
            {
                return Resultado<IReadOnlyCollection<Categoria>>.Falha(
                    ProdutoResource.ErroCategoriaInvalidaOuInativa);
            }

            return Resultado<IReadOnlyCollection<Categoria>>.Sucesso(
                categorias.ToList());
        }
    }
}