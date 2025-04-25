using Atron.Domain.Componentes;
using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public class Modulo
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public ICollection<PerfilDeAcessoModulo> PerfilDeAcessoModulos { get; set; }
        public ICollection<PropriedadeDeFluxoModulo> PropriedadeDeFluxoModulos { get; set; }
    }
}