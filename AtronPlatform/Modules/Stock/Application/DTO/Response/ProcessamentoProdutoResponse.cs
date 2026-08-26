#nullable enable

using AtronStock.Domain.Enums;

namespace AtronStock.Application.DTO.Response;

public sealed class ProcessamentoProdutoResponse
{
    public int Id { get; init; }
    public EStatusProcessamentoProdutoLote Status { get; init; }
    public string CodigoBase { get; init; } = string.Empty;
    public int QuantidadeSolicitada { get; init; }
    public int QuantidadeProcessada { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public string? DescricaoComplementar { get; init; }
    public DateTime DataAquisicao { get; init; }
    public decimal PrecoUnitario { get; init; }
    public IReadOnlyCollection<string> CategoriaCodigos { get; init; } = [];
    public int? LoteProdutoId { get; init; }
    public string? LoteProdutoCodigo { get; init; }
    public string? Erro { get; init; }
}
