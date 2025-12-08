using System.ComponentModel.DataAnnotations;

namespace AtronStock.Domain.Entities
{
    public sealed class ProdutoFornecedor
    {
        [Key] public int Id { get; set; }

        public int ProdutoId { get; set; }

        [MaxLength(25), Required]
        public string ProdutoCodigo { get; set; } = string.Empty;

        public Produto Produto { get; set; } = null!;

        public int FornecedorId { get; set; }

        [MaxLength(25), Required]
        public string FornecedorCodigo { get; set; } = string.Empty;

        public Fornecedor Fornecedor { get; set; } = null!;
    }
}