using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public sealed class Departamento : EntityBase
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }

        public List<Cargo> Cargos { get; set; }
        public List<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }
    }
}