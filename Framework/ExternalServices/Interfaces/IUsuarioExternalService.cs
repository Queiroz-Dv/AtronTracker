using Atron.Application.DTO;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using ExternalServices.Interfaces.ExternalMessage;
using Shared.Interfaces;

namespace ExternalServices.Interfaces
{
    /// <summary>
    /// Interface dos processos e fluxos do módulo de Usuários
    /// </summary>
    public interface IUsuarioExternalService : IApiUri, IExternalMessageService
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
    }
}