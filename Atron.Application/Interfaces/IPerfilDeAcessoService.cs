using Atron.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface IPerfilDeAcessoService
    {
        Task<ICollection<PerfilDeAcessoDTO>> ObterTodosPerfisServiceAsync();
        Task<PerfilDeAcessoDTO> ObterPerfilPorIdServiceAsync(int id);
        Task<PerfilDeAcessoDTO> ObterPerfilPorCodigoServiceAsync(string codigo);
        Task<bool> CriarPerfilServiceAsync(PerfilDeAcessoDTO perfilDeAcessoDTO);
        Task<bool> AtualizarPerfilServiceAsync(string codigo, PerfilDeAcessoDTO perfilDeAcessoDTO);
        Task<bool> DeletarPerfilServiceAsync(string codigo);

        Task<List<PerfilDeAcessoDTO>> ObterPerfisPorCodigoUsuarioServiceAsync(string usuarioCodigo);

        Task<bool> RelacionarPerfilDeAcessoUsuarioServiceAsync(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario);
        Task<PerfilDeAcessoUsuarioDTO> ObterRelacionamentoDePerfilUsuarioPorCodigoServiceAsync(string codigo);
    }
}