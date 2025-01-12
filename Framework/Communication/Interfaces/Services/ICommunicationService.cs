using Shared.Interfaces;
using Shared.Models;

namespace Communication.Interfaces.Services
{
    /// <summary>
    /// Interface de comunicação dos resultados com a API
    /// </summary>
    public interface ICommunicationService : IMessages
    {
        void AddMessage(Message message);

        void AddMessages(List<Message> messages);

        List<Message> GetMessages();
    }
}