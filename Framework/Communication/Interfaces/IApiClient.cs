namespace Communication.Interfaces
{
    /// <summary>
    /// Interface que lida com as requisições do cliente de acordo com o verbo HTTP acionado
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Executa a remoção de um recurso por código
        /// </summary>
        /// <param name="uri">URI que será acionada</param>
        /// <param name="codigo">Código do registro que será removido</param>        
        Task DeleteAsync(string uri, string codigo);

        /// <summary>
        /// Executa a obtenção dos registros
        /// </summary>
        /// <param name="uri">URI que será acionada</param>
        /// <returns>Retorna os registros obtidos</returns>
        Task<string> GetAsync(string uri);

        Task<string> GetAsync(string uri, string parameter);

        /// <summary>
        /// Executa a criação de um registro
        /// </summary>
        /// <param name="uri">URI que será acionada</param>
        /// <param name="content">Conteúdo que será enviado para criação</param>
        Task PostAsync(string uri, string content);

        Task<DTO> PostAsync<DTO>(string url, string content);

        /// <summary>
        /// Executa a ação de atualizar um registro
        /// </summary>
        /// <param name="uri">URI que será acionada</param>
        /// <param name="parameter">Parâmetro que será utilizado como chave para atualizar</param>
        /// <param name="content">Conteúdo que será enviado para atualização</param>        
        Task PutAsync(string uri, string parameter, string content);
    }
}