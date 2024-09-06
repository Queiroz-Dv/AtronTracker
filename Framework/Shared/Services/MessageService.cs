using Shared.Enums;
using Shared.Interfaces;
using Shared.Models;

namespace Shared.Services
{
    public abstract class MessageService : IMessageService
    {
        protected MessageService()
        {
            Messages = new List<Message>();
        }

        public List<Message> Messages { get; }

        public abstract void AddMessage(string message);

        public abstract void AddRegisterNotFoundMessage(string moduleName);

        public abstract void AddRegisterExistMessage(string moduleName);

        public abstract void AddSuccessMessage(string moduleName);

        public abstract void AddRegisterRemovedSuccessMessage(string moduleName);

        public abstract void AddUpdateMessage(string moduleName);

        public abstract void AddRegisterInvalidMessage(string moduleName);

        public abstract void AddError(string message);

        public abstract void AddWarning(string message);

        /// <summary>
        /// Método de automação para inclusão de notificações
        /// </summary>
        /// <param name="description">Mensagem de notificação</param>
        /// <param name="level">Tipo de notificação</param>
        public void AddNotification(string description, MessageLevel level)
        {
            Messages.Add(new Message() { Description = description, Level = level });
        }
    }
}