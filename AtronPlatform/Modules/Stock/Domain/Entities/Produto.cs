using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtronStock.Domain.Entities
{
    public sealed class Produto
    {
        [Key] public int Id { get; set; }

        [MaxLength(25), Required]
        public string Codigo { get; set; } = string.Empty;

        [MaxLength(50), Required]
        public string Descricao { get; set; } = string.Empty;

        [NotMapped]
        public Venda Venda { get; set; } = new();

        [NotMapped]
        public List<Venda> Vendas { get; set; } = [];

        public List<ProdutoCategoria> Categorias { get; set; } = [];

        public List<ProdutoFornecedor> Fornecedores { get; set; } = [];
    }
}