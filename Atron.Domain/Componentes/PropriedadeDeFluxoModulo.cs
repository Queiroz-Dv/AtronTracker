using Atron.Domain.Entities;

namespace Atron.Domain.Componentes
{
    public class PropriedadeDeFluxoModulo
    {
        public int ModuloId { get; set; }
        public string ModuloCodigo { get; set; }

        public int PropriedadeDeFluxoId { get; set; }
        public string PropriedadeDeFluxoCodigo { get; set; }

        public PropriedadesDeFluxo PropriedadesDeFluxo { get; set; }

        public Modulo Modulo { get; set; }
    }
}