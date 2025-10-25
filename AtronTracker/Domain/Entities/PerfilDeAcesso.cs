using System.Collections.Generic;

namespace Domain.Entities
{
    public class PerfilDeAcesso
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }

        public Usuario Usuario { get; set; }

        public ICollection<PerfilDeAcessoModulo> PerfilDeAcessoModulos { get; set; } = new List<PerfilDeAcessoModulo>();
        public ICollection<PerfilDeAcessoUsuario> PerfisDeAcessoUsuario { get; set; }
    }
}