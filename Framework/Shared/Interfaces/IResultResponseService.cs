using Shared.DTO;

namespace Shared.Interfaces
{
    /// <summary>
    /// Interface para o servi�o de resposta
    /// </summary>
    public interface IResultResponseService
    {
        /// <summary>
        /// Lista de resultados que ser�o utilizadas no processo
        /// </summary>
         List<ResultResponseDTO> ResultMessages { get; set; }

        /// <summary>
        /// M�todo que inclu� uma mensagem
        /// </summary>
        /// <param name="message">Mensagem que ser� inclusa</param>
        void AddMessage(string message);


        /// <summary>
        /// M�todo que inclu� um erro
        /// </summary>
        /// <param name="erro">Erro que ser� incluso</param>
        void AddError(string erro);

        /// <summary>
        /// M�todo que inclu� um aviso
        /// </summary>
        /// <param name="warning">Aviso que ser� incluso</param>
        void AddWarning(string warning);
    }
}