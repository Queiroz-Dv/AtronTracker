using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtronStock.Domain.Entities
{
    public class ItemEntrada
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EntradaId { get; set; }

        [ForeignKey("EntradaId")]
        public Entrada Entrada { get; set; } = null!;

        [Required]
        public int ProdutoId { get; set; }

        [Required, MaxLength(25)]
        public string ProdutoCodigo { get; set; } = string.Empty;

        [ForeignKey("ProdutoId")]
        public Produto Produto { get; set; } = null!;

        [Required]
        public int Quantidade { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoCusto { get; set; }
    }
}
