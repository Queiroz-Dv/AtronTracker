namespace Atron.Domain.Entities
{
    public class Salario : EntityBase
    {
        public int DepartmentoId { get; set; }
        public string DepartamentoCodigo { get; set; }
        public Departamento Departmento { get; set; }

        public int CargoId { get; set; }
        public string CargoCodigo { get; set; }
        public Cargo Cargo { get; set; }

        public int UsuarioId { get; set; }
        public string UsuarioCodigo { get; set; }
        public Usuario Usuario { get; set; }

        public int MesId { get; set; }
        public Mes Mes { get; set; }

        public int QuantidadeTotal { get; set; }
        public int Ano { get; set; }
        public int SalarioAntigo { get; set; }
    }
}