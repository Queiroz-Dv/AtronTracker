using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtronStock.Domain.Entities
{
    public class Entrada
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FornecedorId { get; set; }

        [Required, MaxLength(25)]
        public string FornecedorCodigo { get; set; } = string.Empty;

        [ForeignKey("FornecedorId")]
        public Fornecedor Fornecedor { get; set; } = null!;

        [Required]
        public DateTime DataEntrada { get; set; }

        [MaxLength(500)]
        public string? Observacao { get; set; }

        public List<ItemEntrada> Itens { get; set; } = new();
    }
}
