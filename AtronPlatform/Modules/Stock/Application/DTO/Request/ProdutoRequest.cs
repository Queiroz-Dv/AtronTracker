#nullable enable

using System.ComponentModel.DataAnnotations;

namespace AtronStock.Application.DTO.Request
{
    public sealed class ProdutoRequest
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public string Descricao { get; set; } = string.Empty;

        public string? DescricaoComplementar { get; set; }

        [Required]
        public DateTime DataAquisicao { get; set; }

        public decimal PrecoUnitario { get; set; }

        public List<string> CategoriaCodigos { get; set; } = [];

    }
}
