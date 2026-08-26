using AtronStock.Application.DTO.Request;
using AtronStock.Domain.Entities;

namespace AtronStock.Application.Mapping;

public sealed record ProdutoCriacaoMappingInput(
    ProdutoRequest Request,
    IReadOnlyCollection<Categoria> Categorias);

public sealed record ProdutoAtualizacaoMappingInput(
    ProdutoAtualizacaoRequest Request,
    IReadOnlyCollection<Categoria> Categorias);
