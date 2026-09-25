namespace AtronStock.Domain.Entities
{
    public class ProdutoCategoria
    {
        public int ProdutoId { get; set; }
        public string ProdutoCodigo { get; set; } = string.Empty;
        public Produto Produto { get; set; } = null!;
        public int CategoriaId { get; set; }
        public string CategoriaCodigo { get; set; } = string.Empty;
        public Categoria Categoria { get; set; } = null!;
    }
}
