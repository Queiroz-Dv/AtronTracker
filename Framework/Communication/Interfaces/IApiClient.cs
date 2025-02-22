using Communication.Interfaces.Services;

namespace Communication.Interfaces
{
    /// <summary>
    /// Interface que lida com as requisições do cliente de acordo com o verbo HTTP acionado
    /// </summary>
    public interface IApiClient : IUrlTransferService
    {
        /// <summary>
        /// Executa a remoção de um recurso por código
        /// </summary>        
        /// <param name="codigo">Código do registro que será removido</param>        
        Task DeleteAsync(string codigo);

        /// <summary>
        /// Executa a obtenção dos registros
        /// </summary>        
        /// <returns>Retorna os registros obtidos</returns>
        Task<string> GetAsync();

        Task<string> GetAsync(string parameter);

        Task<string> GetAsync(int parameter);
        Task<DTO> GetAsync<DTO>(string parameter);

        /// <summary>
        /// Executa a criação de um registro
        /// </summary>        
        /// <param name="content">Conteúdo que será enviado para criação</param>
        Task PostAsync(string content);

        Task<DTO> PostAsync<DTO>(string content);

        /// <summary>
        /// Executa a ação de atualizar um registro
        /// </summary>
        /// <param name="parameter">Parâmetro que será utilizado como chave para atualizar</param>
        /// <param name="content">Conteúdo que será enviado para atualização</param>        
        Task PutAsync(string parameter, string content);

        Task PutAsyncById(int parameter, string content);
    }
}