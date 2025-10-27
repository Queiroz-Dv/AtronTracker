using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtronStock.Domain.Entities
{
    public sealed class Venda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DataVenda { get; set; }

        [Required]
        public int QuantidadeDeProdutoVendido { get; set; }

        [Required]
        public decimal PrecoDoProduto { get; set; }

        public bool Removido { get; set; }

        public DateTime? RemovidoEm { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public string ProdutoCodigo { get; set; } = string.Empty;   

        public List<Produto> Produtos { get; set; } = [];

        [ForeignKey(nameof(ProdutoCodigo))]
        public Produto Produto { get; set; } = new();

        [Required]
        public int CategoriaId { get; set; }

        [Required]
        public string CategoriaCodigo { get; set; } = string.Empty;

        [ForeignKey(nameof(CategoriaCodigo))]
        public Categoria Categoria { get; set; } = new();

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public string ClienteCodigo { get; set; } = string.Empty;

        [ForeignKey(nameof(ClienteCodigo))]
        public Cliente Cliente { get; set; } = new();

    }
}