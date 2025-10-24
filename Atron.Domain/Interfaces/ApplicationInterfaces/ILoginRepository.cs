using Atron.Tracker.Domain.Entities;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.ApplicationInterfaces
{
    public interface ILoginRepository
    {
        Task<bool> AtualizarSenhaUsuario(string codigoDoUsuario, string senha);

        Task<bool> AutenticarUsuarioAsync(UsuarioIdentity usuarioIdentity);
        
        Task Logout();
    }
}