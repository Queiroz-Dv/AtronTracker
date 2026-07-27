using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtronStock.Domain.Entities
{
    public class Estoque
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public Produto Produto { get; set; } = null!;

        [Required]
        public int Quantidade { get; set; }

        public DateTime DataUltimaAtualizacao { get; set; }
    }
}
