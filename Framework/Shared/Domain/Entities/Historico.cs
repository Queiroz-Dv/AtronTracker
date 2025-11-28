using System.ComponentModel.DataAnnotations;

namespace Shared.Domain.Entities
{
    public sealed class Historico
    {
        [Key]
        public int Id { get; set; }

        public long CodigoHistorico { get; set; }

        [Required, MaxLength(50)]
        public string Contexto { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string CodigoRegistro { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(1500)]
        public string Descricao { get; set; } = string.Empty;
    }
}