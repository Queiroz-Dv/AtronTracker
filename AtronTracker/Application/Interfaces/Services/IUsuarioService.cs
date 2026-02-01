using Application.DTO;
using Application.DTO.Request;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        /// <summary>
        /// Obtém todos os usuários como UsuarioDTO (para compatibilidade com LoginService).
        /// </summary>
        Task<List<UsuarioDTO>> ObterTodosAsync();

        /// <summary>
        /// Obtém um usuário por código como UsuarioDTO (para compatibilidade com LoginService).
        /// </summary>
        Task<UsuarioDTO> ObterPorCodigoAsync(string codigo);

        /// <summary>
        /// Obtém todos os usuários como UsuarioRequest (padrão CategoriaService).
        /// </summary>
        Task<Resultado<ICollection<UsuarioRequest>>> ObterTodosRequestAsync();

        /// <summary>
        /// Obtém um usuário por código como UsuarioRequest (padrão CategoriaService).
        /// </summary>
        Task<Resultado<UsuarioDTO>> ObterPorCodigoRequestAsync(string codigo);

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        Task<Resultado> CriarAsync(UsuarioRequest request);

        /// <summary>
        /// Atualiza um usuário existente (padrão CategoriaService).
        /// </summary>
        Task<Resultado> AtualizarAsync(UsuarioRequest request);

        /// <summary>
        /// Remove um usuário pelo código.
        /// </summary>
        Task<Resultado> RemoverAsync(string codigo);
    }
}