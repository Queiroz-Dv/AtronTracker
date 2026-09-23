using Domain.Attributes;
using Domain.Constants;
using Domain.Interfaces;
using System.Collections.Generic;

namespace Domain.Entities
{
    [TenantModule(TrackerModulos.Cargo)]
    public sealed class Cargo : ITenantScoped
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public int DepartamentoId { get; set; }
        public string DepartamentoCodigo { get; set; }

        public Departamento Departamento { get; set; }

        public List<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }

        public List<PlanejamentoCustoCargo> PlanejamentosCustoCargo { get; set; }

        public Cargo VincularDepartamento(Departamento departamento)
        {
            DepartamentoId = departamento.Id;
            DepartamentoCodigo = departamento.Codigo;
            return this;
        }
    }
}
