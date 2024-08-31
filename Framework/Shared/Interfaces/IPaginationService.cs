using Shared.DTO;

namespace Shared.Interfaces
{
    /// <summary>
    /// Interface para os serviços e processos de paginação
    /// </summary>
    /// <typeparam name="T">Entidade que é utilizada no processo.</typeparam>
    public interface IPaginationService<T>
    {
        /// <summary>
        /// Método que realiza a paginação dos itens
        /// </summary>
        /// <param name="items">Itens que serão paginados</param>
        /// <param name="itemPage">Item da página atual</param>
        /// <param name="controllerName">Nome do controlador utilizado no processo</param>
        /// <param name="filter">Filtro que será utilizado no processo interno</param>
        /// <param name="action">Nome da ação que será utilizada internamente onde por padrão é a index</param>
        void Paginate(List<T> items, int itemPage, string controllerName, string filter, string action = nameof(Index));
        
        /// <summary>
        /// Configura a entidade para ser paginada automaticamente
        /// </summary>
        /// <param name="items">Itens que serão paginados</param>
        /// <param name="filter">Filtro de busca que será utilizado no processo </param>
        void ConfigureEntityPaginated(List<T> items, string filter);

        /// <summary>
        /// Obtém as entidades totalmente paginadas e atualizadas
        /// </summary>
        /// <returns>Retorna uma lista de entidades paginadas de acordo com as configurações da paginação</returns>
        List<T> GetEntitiesFilled();

        /// <summary>
        /// Propriedade de Paginação que será enviada para alimentar as views com as informações necessárias
        /// </summary>
        PageInfoDTO PageInfo { get; }

        /// <summary>
        /// Filtragem por algum valor chave da entidade
        /// </summary>
        string FilterBy { get; set; }

        /// <summary>
        /// Força a filtragem externa do que por passagem de parâmetros
        /// </summary>
        bool ForceFilter { get; set; }
    }
}