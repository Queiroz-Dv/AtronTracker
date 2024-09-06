using Shared.Models;

namespace Shared.Interfaces
{
    public interface IMessageModelService
    {        
        public IList<Message> GetMessages();
    }
}