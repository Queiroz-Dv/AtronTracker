using AtronStock.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtronStock.Domain.Entities
{
    public sealed class Historico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AuditoriaId { get; set; }

        [ForeignKey(nameof(AuditoriaId))]
        public Auditoria Auditoria { get; set; } = new Auditoria();

        [Required, MaxLength(1500)]
        public string Descricao { get; set; } = string.Empty;
    }
}