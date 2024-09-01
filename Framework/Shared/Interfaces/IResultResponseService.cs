using Shared.DTO;

namespace Shared.Interfaces
{
    /// <summary>
    /// Interface para o serviço de resposta
    /// </summary>
    public interface IResultResponseService
    {
        /// <summary>
        /// Lista de resultados que serão utilizadas no processo
        /// </summary>
         List<ResultResponseDTO> ResultMessages { get; set; }

        /// <summary>
        /// Método que incluí uma mensagem
        /// </summary>
        /// <param name="message">Mensagem que será inclusa</param>
        void AddMessage(string message);


        /// <summary>
        /// Método que incluí um erro
        /// </summary>
        /// <param name="erro">Erro que será incluso</param>
        void AddError(string erro);

        /// <summary>
        /// Método que incluí um aviso
        /// </summary>
        /// <param name="warning">Aviso que será incluso</param>
        void AddWarning(string warning);
    }
}