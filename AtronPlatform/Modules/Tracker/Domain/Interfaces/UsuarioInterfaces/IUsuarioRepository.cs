using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.UsuarioInterfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> ObterUsuariosAsync();
        Task<Usuario> ObterUsuarioPorIdAsync(int? id);
        Task<Usuario> ObterUsuarioPorCodigoAsync(string codigo);
        Task<Usuario> ObterUsuarioGeralPorCodigoAsync(string codigo);
        Task<Usuario> ObterInativoPorEmailAsync(string email);
        Task<Usuario> ObterUsuarioGeralPorEmailAsync(string email);
        Task<bool> CriarUsuarioAsync(Usuario usuario);
        Task<bool> AtualizarUsuarioAsync(Usuario usuario);
        Task<bool> AtualizarPreferenciaNotificacaoTarefaPorEmailAsync(string codigo, bool receberNotificacao);
        Task<bool> AtualizarPreferenciasNotificacaoTarefaAsync(string codigo, bool receberNotificacaoInterna, bool receberNotificacaoPorEmail);
        Task<bool> ConfirmarEmailAsync(string codigo);
        Task<bool> RemoverUsuarioAsync(Usuario usuario);
        Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity();
        Task<bool> VerificarEmailExistenteAsync(string email);
    }
}
