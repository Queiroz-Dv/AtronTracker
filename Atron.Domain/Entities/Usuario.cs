using System;

namespace Atron.Domain.Entities
{
    public class Usuario : EntityBase
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public int DepartamentoId { get; set; }
        public string DepartamentoCodigo { get; set; }
        public int CargoId { get; set; }
        public string CargoCodigo { get; set; }
        public int Salario { get; set; }
        public DateTime? DataNascimento { get; set; }
    }
}