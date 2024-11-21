using Atron.Application.DTO;

namespace ExternalServices.Interfaces
{
    /// <summary>
    /// Interface dos processos e fluxos do módulo de Usuários
    /// </summary>
    public interface IUsuarioExternalService
    {
        /// <summary>
        /// Método que cria um usuário
        /// </summary>
        /// <param name="usuarioDTO">DTO que será enviado para criação</param>
        Task Criar(UsuarioDTO usuarioDTO);

        /// <summary>
        /// Método que obtém todos os usuários
        /// </summary>
        /// <returns>Retorna uma lista de usuários</returns>
        Task<List<UsuarioDTO>> ObterTodos();


        Task<UsuarioDTO> ObterPorCodigo(string codigo);

        Task Atualizar(string codigoUsuario, UsuarioDTO usuario);

        Task Remover(string codigo);
    }
}