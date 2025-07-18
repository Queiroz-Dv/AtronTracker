using Atron.Domain.Customs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Atron.Domain.Entities
{
    [ModuloInfo("CRG", nameof(Cargo))]
    public sealed class Cargo
    {
        [Key] public int Id { get; set; }
        [MaxLength(10)] public string Codigo { get; set; }
        [MaxLength(50)] public string Descricao { get; set; }

        [ForeignKey(nameof(DepartamentoId))]
        [NotNull] public int DepartamentoId { get; set; }

        [ForeignKey(nameof(DepartamentoCodigo))]
        [NotNull][MaxLength(10)] public string DepartamentoCodigo { get; set; }

        public Departamento Departamento { get; set; }

        public List<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }
    }
}