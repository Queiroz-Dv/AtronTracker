using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public sealed class Departamento : EntityBase
    {
        public string Codigo { get; private set; }
        public string Descricao { get; private set; }

        public void SetDescricao(string descricao)
        {
            Descricao = descricao;
        }

        public List<Cargo> Cargos { get;  set; }
    }
}