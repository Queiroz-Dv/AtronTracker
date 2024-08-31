using Shared.DTO;
using Shared.Enums;
using Shared.Interfaces;

namespace Shared.Services
{
    /// <summary>
    /// Classe de serviço com as definições dos processos de resultados das respostas da API
    /// </summary>
    public abstract class ResultResponseService : IResultResponseService
    {
        protected ResultResponseService()
        {
            ResultMessages = new List<ResultResponseDTO>();
        }

        /// <summary>
        /// Método que incluí uma notificação de acordo com a mensagem e nível informados
        /// </summary>
        /// <param name="message">Mensagem que será incluída</param>
        /// <param name="level">Nível da mensagem</param>
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