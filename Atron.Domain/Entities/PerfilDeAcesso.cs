using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public class PerfilDeAcesso
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public ICollection<Modulo> Modulos { get; set; }
        public ICollection<Usuario> Usuarios { get; set; }
    }
}