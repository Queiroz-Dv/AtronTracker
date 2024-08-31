using Shared.DTO;
using Shared.Enums;
using Shared.Interfaces;

namespace Shared.Services
{
    /// <summary>
    /// Classe de servi�o com as defini��es dos processos de resultados das respostas da API
    /// </summary>
    public abstract class ResultResponseService : IResultResponseService
    {
        protected ResultResponseService()
        {
            ResultMessages = new List<ResultResponseDTO>();
        }

        /// <summary>
        /// M�todo que inclu� uma notifica��o de acordo com a mensagem e n�vel informados
        /// </summary>
        /// <param name="message">Mensagem que ser� inclu�da</param>
        /// <param name="level">N�vel da mensagem</param>
        public void AddNotification(string message, ResultResponseLevelEnum level)
        {
            ResultMessages.Add(new ResultResponseDTO() { Message = message, Level = level });
        }

        public List<ResultResponseDTO> ResultMessages { get; set; }

        public abstract void AddMessage(string message);

        public abstract void AddError(string message);

        public abstract void AddWarning(string message);
    }
}