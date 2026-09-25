using AtronStock.Domain.Constants;
using AtronStock.Domain.Enums;
using Shared.Attributes;
using Shared.Domain.Entities.Identity;

namespace AtronStock.Domain.Entities
{
    [TenantModule(StockModulos.Categoria)]
    public sealed class Categoria : ITenantScoped
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public EStatus Status { get; set; }
        public List<ProdutoCategoria> Produtos { get; set; } = [];
    }
}