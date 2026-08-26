#nullable enable

using AtronStock.Domain.Entities;

namespace AtronStock.Application.DTO.Request;

public sealed record GeracaoProdutosLoteCommand(
    string CodigoBase,
    int Quantidade,
    string Descricao,
    string? DescricaoComplementar,
    DateTime DataAquisicao,
    decimal PrecoUnitario,
    IReadOnlyCollection<string> CategoriaCodigos)
{
    public static GeracaoProdutosLoteCommand Criar(ProcessamentoProdutoLote processamento)
        => new(
            processamento.Solicitacao.CodigoBase,
            processamento.Solicitacao.QuantidadeSolicitada,
            processamento.Solicitacao.Descricao,
            processamento.Solicitacao.DescricaoComplementar,
            processamento.Solicitacao.DataAquisicao,
            processamento.Solicitacao.PrecoUnitario,
            processamento.Solicitacao.CategoriaCodigos);

    public static GeracaoProdutosLoteCommand Criar(GerarProdutosLoteRequest request)
        => new(
            request.CodigoBase,
            request.Quantidade,
            request.Descricao,
            request.DescricaoComplementar,
            request.DataAquisicao,
            request.PrecoUnitario,
            request.CategoriaCodigos ?? []);
}
