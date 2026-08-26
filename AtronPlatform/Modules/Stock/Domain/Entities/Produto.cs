using System.ComponentModel.DataAnnotations;
using AtronStock.Domain.Enums;

namespace AtronStock.Domain.Entities
{
    public sealed class Produto
    {
        [Key] public int Id { get; set; }

        [MaxLength(25), Required]
        public string Codigo { get; set; } = string.Empty;

        [MaxLength(50), Required]
        public string Descricao { get; set; } = string.Empty;

        public string? DescricaoComplementar { get; set; }

        public DateTime? DataAquisicao { get; set; }

        public decimal? PrecoUnitario { get; set; }

        public DateTime? DataEfetivaBaixa { get; set; }

        public EStatusProduto Status { get; set; } = EStatusProduto.Ativo;

        public int? LoteProdutoId { get; set; }

        public LoteProduto? LoteProduto { get; set; }

        public List<ProdutoCategoria> Categorias { get; set; } = [];

    }
}
