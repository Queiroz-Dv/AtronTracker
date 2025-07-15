using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.UsuarioInterfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<IEnumerable<Usuario>> ObterUsuariosAsync();

        Task<Usuario> ObterUsuarioPorIdAsync(int? id);

        Task<UsuarioIdentity> ObterUsuarioPorCodigoAsync(string codigo);

        Task<bool> CriarUsuarioAsync(Usuario usuario);

        Task<bool> AtualizarUsuarioAsync(string codigo, Usuario usuario);

        Task<bool> RemoverUsuarioAsync(Usuario usuario);

        bool UsuarioExiste(string codigo);

        Task<bool> AtualizarSalario(int usuarioId, int quantidadeTotal);

        Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity();
    }
}