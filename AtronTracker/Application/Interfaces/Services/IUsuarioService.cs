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
        /// Obtém todos os usuários
        /// </summary>
        /// <summary>
        /// Obtém todos os usuários
        /// </summary>
        Task<Resultado<List<UsuarioDTO>>> ObterTodosAsync();

        /// <summary>
        /// Obtém um usuário por código
        /// </summary>
        Task<Resultado<UsuarioDTO>> ObterPorCodigoAsync(string codigo);

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        Task<Resultado<UsuarioRequest>> CriarAsync(UsuarioRequest request);

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        Task<Resultado<UsuarioRequest>> AtualizarAsync(UsuarioRequest request);

        /// <summary>
        /// Remove um usuário pelo código.
        /// </summary>
        Task<Resultado> RemoverAsync(string codigo);
    }
}