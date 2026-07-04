using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace Domain.Entities
{
    public sealed class PlanejamentoCusto
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(10)]
        [NotNull]
        public string Codigo { get; set; }

        [MaxLength(100)]
        [NotNull]
        public string Descricao { get; set; }

        public int Ano { get; set; }

        public decimal ValorMinimo { get; set; }

        public decimal ValorTeto { get; set; }

        public bool ApenasDepartamento { get; set; }

        [ForeignKey(nameof(DepartamentoId))]
        [NotNull]
        public int DepartamentoId { get; set; }

        [ForeignKey(nameof(DepartamentoCodigo))]
        [NotNull]
        [MaxLength(10)]
        public string DepartamentoCodigo { get; set; }

        public Departamento Departamento { get; set; }

        public List<PlanejamentoCustoCargo> DetalhesCargo { get; set; } = [];

        public PlanejamentoCusto VincularDepartamento(Departamento departamento)
        {
            DepartamentoId = departamento.Id;
            DepartamentoCodigo = departamento.Codigo;
            return this;
        }
    }
}
