using Atron.Domain.Customs;
using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    [ModuloInfo("PRF", "Perfil de Acesso")]
    public class PerfilDeAcesso
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }

        public Usuario Usuario { get; set; }

        public ICollection<PerfilDeAcessoModulo> PerfilDeAcessoModulos { get; set; }
        public ICollection<PerfilDeAcessoUsuario> PerfisDeAcessoUsuario { get; set; }
    }
}