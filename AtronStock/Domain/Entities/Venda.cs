using AtronStock.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtronStock.Domain.Entities
{
    public sealed class Venda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DataVenda { get; set; }

        public bool Removido { get; set; }

        public DateTime? RemovidoEm { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required, MaxLength(25)]
        public string ClienteCodigo { get; set; } = string.Empty;

        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = null!;

        public List<ItemVenda> Itens { get; set; } = new();
    }
}