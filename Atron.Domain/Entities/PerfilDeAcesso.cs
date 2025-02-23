using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public class PerfilDeAcesso
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }
        public ICollection<PerfilDeAcessoModulo> PerfilDeAcessoModulos { get; set; } = new List<PerfilDeAcessoModulo>();
        public ICollection<PerfilDeAcessoUsuario> PerfilDeAcessoUsuarios { get; set; } = new List<PerfilDeAcessoUsuario>();
    }
}