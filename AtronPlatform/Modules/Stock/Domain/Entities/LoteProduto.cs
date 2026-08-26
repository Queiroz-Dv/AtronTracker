using System.ComponentModel.DataAnnotations;

namespace AtronStock.Domain.Entities
{
    public sealed class LoteProduto
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Codigo { get; set; } = string.Empty;

        public List<Produto> Produtos { get; set; } = [];
    }
}
