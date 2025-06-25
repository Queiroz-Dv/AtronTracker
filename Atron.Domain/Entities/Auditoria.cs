using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atron.Domain.Entities
{
    public sealed class Auditoria
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public DateTime? DataAlteracao { get; set; }

        [Required, MaxLength(25)]
        public string CriadoPor { get; set; } // Código do usuário 

        [MaxLength(25)]
        public string AlteradoPor { get; set; }

        [Required, MaxLength(10)]
        public string ModuloCodigo { get; set; }  // ex: "DPT", "USR"

        public bool? Inativo { get; set; } = false;

        public DateTime? RemovidoEm { get; set; }

        // Relacionamento 1:N com histórico
        public List<Historico> Historicos { get; set; } = new();
    }
}