using System.ComponentModel.DataAnnotations;

namespace AtronStock.Application.DTO.Request
{
    public class VendaRequest
    {
        [Required]
        public int ClienteId { get; set; }

        [Required]
        public List<ItemVendaRequest> Itens { get; set; } = new();
    }

    public class ItemVendaRequest
    {
        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public decimal PrecoVenda { get; set; }
    }
}
