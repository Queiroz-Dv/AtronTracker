using System.ComponentModel.DataAnnotations;

namespace AtronStock.Domain.Entities
{
    public class ProdutoCategoria
    {
        public int ProdutoId { get; set; }

        [MaxLength(25), Required]
        public string ProdutoCodigo { get; set; } = string.Empty;

        public Produto Produto { get; set; } = new();

        public int CategoriaId { get; set; }

        [MaxLength(25), Required]
        public string CategoriaCodigo { get; set; } = string.Empty;
        public Categoria Categoria { get; set; } = new();
    }
}