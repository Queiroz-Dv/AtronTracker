using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atron.Domain.Entities
{
    public sealed class Cargo
    {
        public Cargo() { }

        public Cargo(string codigo, string descricao, string departamentoCodigo)
        {
            Codigo = codigo.ToUpper();
            Descricao = descricao.ToUpper();
            DepartamentoCodigo = departamentoCodigo;
        }

        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }

        [NotMapped]
        public int DepartamentoId_Antigo { get; set; }

        public int DepartmentoId { get; set; }

        public string DepartamentoCodigo { get; set; }

        public Departamento Departamento { get; set; }

        public List<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }
    }
}