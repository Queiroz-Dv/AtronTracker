using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Atron.Domain.ValueObjects
{
    [Owned]
    public class NomeCompleto
    {
        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [MaxLength(25)]
        [MinLength(3)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Sobrenome é obrigatório")]
        [MaxLength(25)]
        [MinLength(3)]
        public string Sobrenome { get; set; }

        public override string ToString() => string.Concat(Nome, Sobrenome);
    }
}