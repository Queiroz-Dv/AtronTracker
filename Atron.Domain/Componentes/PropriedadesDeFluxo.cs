using System.Collections.Generic;

namespace Atron.Domain.Componentes
{
    public class PropriedadesDeFluxo
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public ICollection<PropriedadeDeFluxoModulo> PropriedadesDeFluxoModulo { get; set; }
    }
}