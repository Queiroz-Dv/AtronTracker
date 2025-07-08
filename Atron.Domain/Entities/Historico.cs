using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atron.Domain.Entities
{
    public sealed class Historico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AuditoriaId { get; set; }

        [ForeignKey(nameof(AuditoriaId))]
        public Auditoria Auditoria { get; set; }

        [Required, MaxLength(1500)]
        public string Descricao { get; set; }  // ex: "Venda criada com 3 itens"        
    }
}