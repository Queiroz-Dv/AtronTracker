namespace Atron.Domain.Entities
{
    public sealed class Cargo : EntityBase
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }

        public int DepartamentoId_Antigo { get; set; }

        public int DepartmentoId { get; set; }

        public string DepartmentoCodigo { get; set; }

        public Departamento Departmento { get; set; }
    }
}