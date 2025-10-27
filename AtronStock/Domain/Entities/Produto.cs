using System.ComponentModel.DataAnnotations;

namespace AtronStock.Domain.Entities
{
    public sealed class Produto
    {
        [Key] public int Id { get; set; }

        [MaxLength(25), Required]
        public string Codigo { get; set; } = string.Empty;

        [MaxLength(50), Required]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public int QuantidadeEmEstoque { get; set; }

        public bool Removido { get; set; }

        public DateTime? RemovidoEm { get; set; }

        public Venda Venda { get; set; } = new();

        public List<Venda> Vendas { get; set; } = [];

        public List<ProdutoCategoria> Categorias { get; set; } = [];
    }
}