using AtronStock.Domain.Constants;
using AtronStock.Domain.Enums;
using Shared.Attributes;
using Shared.Domain.Entities.Identity;

namespace AtronStock.Domain.Entities
{
    [TenantModule(StockModulos.Produto)]
    public sealed class Produto : ITenantScoped
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

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
