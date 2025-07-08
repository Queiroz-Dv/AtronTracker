using System.ComponentModel.DataAnnotations;

namespace Atron.Domain.Entities
{
    public class ProdutoCategoria
    {
        public int ProdutoId { get; set; }

        [MaxLength(25), Required]
        public string ProdutoCodigo { get; set; }

        public Produto Produto { get; set; }

        public int CategoriaId { get; set; }

        [MaxLength(25), Required]
        public string CategoriaCodigo { get; set; }
        public Categoria Categoria { get; set; }
    }
}