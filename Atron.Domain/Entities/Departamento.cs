using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public sealed class Departamento : EntityBase
    {
        public Departamento() { }

        public Departamento(string codigo, string descricao)
        {
            Codigo = codigo.ToUpper();
            Descricao = descricao.ToUpper();
        }

        public string Codigo { get; set; }
        public string Descricao { get; set; }

        public List<Cargo> Cargos { get; set; }
        public List<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }
    }
}