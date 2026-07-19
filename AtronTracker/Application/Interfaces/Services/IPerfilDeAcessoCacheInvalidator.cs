using Domain.Entities;
using System.Collections.Generic;

namespace Application.Interfaces.Services
{
    public interface IPerfilDeAcessoCacheInvalidator
    {
        void InvalidarUsuarios(IEnumerable<string> codigosUsuarios);

        void InvalidarUsuariosDoPerfil(PerfilDeAcesso perfil);
    }
}
