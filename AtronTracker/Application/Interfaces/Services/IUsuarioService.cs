using Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDTO>> ObterTodosAsync();

        Task<UsuarioDTO> ObterPorCodigoAsync(string codigo);

        Task<UsuarioDTO> CriarAsync(UsuarioDTO usuarioDTO);

        Task AtualizarAsync(string codigo, UsuarioDTO usuarioDTO);

        Task RemoverAsync(string codigo);
    }
}