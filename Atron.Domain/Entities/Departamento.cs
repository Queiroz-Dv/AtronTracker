using Atron.Domain.Customs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Atron.Domain.Entities
{
    [ModuloInfo("DPT", nameof(Departamento))]
    public sealed class Departamento
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(10)][NotNull] public string Codigo { get; set; }
        [MaxLength(50)][NotNull] public string Descricao { get; set; }

        public List<Cargo> Cargos { get; set; }
        public List<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }
    }
}