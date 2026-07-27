using System.ComponentModel.DataAnnotations;

namespace AtronStock.Application.DTO.Request
{
    public class EntradaRequest
    {
        [Required]
        public int FornecedorId { get; set; }

        [MaxLength(25), Required]
        public string FornecedorCodigo { get; set; }

        [Required]
        public List<ItemEntradaRequest> Itens { get; set; } = new();
    }

    public class ItemEntradaRequest
    {
        [Required]
        public int ProdutoId { get; set; }

        public string ProdutoCodigo { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public decimal PrecoCusto { get; set; }
    }
}
