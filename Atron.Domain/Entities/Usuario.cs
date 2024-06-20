using Atron.Domain.ValueObjects;
using System;

namespace Atron.Domain.Entities
{
    public class Usuario : EntityBase
    {
        public long CodigoUsuario { get; set; }
        public NomeCompleto NomeCompleto { get; set; }
        public int DepartmentoId { get; set; }
        public string DepartmentoCodigo { get; set; }
        public Departamento Departmento { get; set; }
        public int CargoId { get; set; }
        public string CargoCodigo { get; set; }
        public Cargo Cargo { get; set; }
        public int Salario { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Senha { get; set; }
    }
}