#nullable enable

using System.ComponentModel.DataAnnotations;

namespace AtronStock.Application.DTO.Request;

public sealed class GerarProdutosLoteRequest
{
    [Required]
    public string CodigoBase { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    [Required]
    public string Descricao { get; set; } = string.Empty;

    public string? DescricaoComplementar { get; set; }

    [Required]
    public DateTime DataAquisicao { get; set; }

    public decimal PrecoUnitario { get; set; }

    public List<string> CategoriaCodigos { get; set; } = [];
}
