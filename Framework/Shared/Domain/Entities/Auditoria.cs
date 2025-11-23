using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Domain.Entities
{
    public sealed class Auditoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public DateTime? DataAlteracao { get; set; }

        [Required, MaxLength(25)]
        public string CriadoPor { get; set; }  = string.Empty;

        [MaxLength(25)]
        public string AlteradoPor { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string CodigoRegistro { get; set; }        

        public DateTime? RemovidoEm { get; set; }

        [NotMapped]
        public List<Historico> Historicos { get; set; } = [];
    }
}