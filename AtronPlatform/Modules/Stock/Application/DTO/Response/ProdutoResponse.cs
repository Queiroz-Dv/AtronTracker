#nullable enable

using AtronStock.Domain.Enums;

namespace AtronStock.Application.DTO.Response
{
    public sealed class ProdutoResponse
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? DescricaoComplementar { get; set; }
        public DateTime? DataAquisicao { get; set; }
        public decimal? PrecoUnitario { get; set; }
        public DateTime? DataEfetivaBaixa { get; set; }
        public EStatusProduto Status { get; set; }
        public int? LoteProdutoId { get; set; }
        public string? LoteProdutoCodigo { get; set; }
        public List<CategoriaProdutoResponse> Categorias { get; set; } = [];
    }

    public sealed class CategoriaProdutoResponse
    {
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }
}
