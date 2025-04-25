using System.Collections.Generic;

namespace Atron.Application.DTO
{
    public class PerfilDeAcessoUsuarioDTO
    {
        public PerfilDeAcessoUsuarioDTO()
        {
            PerfilDeAcesso = new PerfilDeAcessoDTO();
            Usuarios = new List<UsuarioDTO>();
        }

        public PerfilDeAcessoDTO PerfilDeAcesso { get; set; } 
        public ICollection<UsuarioDTO> Usuarios { get; set; }
    }
}