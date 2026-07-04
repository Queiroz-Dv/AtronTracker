using Application.DTO;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPerfilDeAcessoService
    {
        Task<Resultado<List<PerfilDeAcessoDTO>>> ObterTodosAsync();

        Task<Resultado<PerfilDeAcessoDTO>> ObterPorCodigoAsync(string codigo);

        Task<Resultado<PerfilDeAcessoDTO>> CriarAsync(PerfilDeAcessoDTO perfilDeAcessoDTO);

        Task<Resultado<PerfilDeAcessoDTO>> AtualizarAsync(string codigo, PerfilDeAcessoDTO perfilDeAcessoDTO);

        Task<Resultado> RemoverAsync(string codigo);

        Task<Resultado<PerfilDeAcessoUsuarioDTO>> RelacionarPerfilDeAcessoUsuarioAsync(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario);

        Task<Resultado<PerfilDeAcessoUsuarioDTO>> ObterRelacionamentoDePerfilUsuarioPorCodigoAsync(string codigo);

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
