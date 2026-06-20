using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.UsuarioInterfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<IEnumerable<Usuario>> ObterUsuariosAsync();
        Task<Usuario> ObterUsuarioPorIdAsync(int? id);
        Task<Usuario> ObterUsuarioPorCodigoAsync(string codigo);
        Task<Usuario> ObterUsuarioGeralPorCodigoAsync(string codigo);
        Task<Usuario> ObterInativoPorEmailAsync(string email);
        Task<Usuario> ObterUsuarioGeralPorEmailAsync(string email);
        Task<bool> CriarUsuarioAsync(Usuario usuario);
        Task<bool> AtualizarUsuarioAsync(Usuario usuario);
        Task<bool> RemoverUsuarioAsync(Usuario usuario);
        Task<bool> AtualizarSalario(int usuarioId, int quantidadeTotal);
        Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity();
        Task<bool> VerificarEmailExistenteAsync(string email);
    }
}