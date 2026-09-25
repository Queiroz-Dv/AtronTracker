using Domain.Constants;
using Shared.Attributes;
using Shared.Domain.Entities.Identity;
using System.Collections.Generic;

namespace Domain.Entities
{
    [TenantModule(TrackerModulos.Departamento)]
    public sealed class Departamento : ITenantScoped
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public int? GestorDepartamentoId { get; set; }
        public string GestorDepartamentoCodigo { get; set; }
        public Usuario GestorDepartamento { get; set; }

        public List<Cargo> Cargos { get; set; }
        public List<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }
        public List<PlanejamentoCusto> PlanejamentosCusto { get; set; }
    }
}