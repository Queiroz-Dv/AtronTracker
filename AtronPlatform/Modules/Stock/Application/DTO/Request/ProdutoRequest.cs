using System.ComponentModel.DataAnnotations;

namespace AtronStock.Application.DTO.Request
{
    public class ProdutoRequest
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public string Descricao { get; set; } = string.Empty;

        public decimal Preco { get; set; }

        public List<string> CategoriaCodigos { get; set; } = [];

        public List<string> FornecedoresCodigos { get; set; } = [];
    }

    public class ProdutoBulkRequest : ProdutoRequest
    {
        [Required]
        public int Quantidade { get; set; }
    }
}
