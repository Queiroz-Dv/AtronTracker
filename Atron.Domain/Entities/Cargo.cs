using System.ComponentModel.DataAnnotations.Schema;

namespace Atron.Domain.Entities
{
    public sealed class Cargo : EntityBase
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }

        [NotMapped]
        public int DepartamentoId_Antigo { get; set; }

        public int DepartmentoId { get; set; }

        public string DepartmentoCodigo { get; set; }

        public Departamento Departamento { get; set; }
    }
}