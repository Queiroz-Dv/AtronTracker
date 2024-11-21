using Atron.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDTO>> ObterTodosAsync();

        Task<UsuarioDTO> ObterPorCodigoAsync(string codigo);

        Task CriarAsync(UsuarioDTO usuarioDTO);

        Task AtualizarAsync(UsuarioDTO usuarioDTO);

        Task RemoverAsync(string codigo);
    }
}