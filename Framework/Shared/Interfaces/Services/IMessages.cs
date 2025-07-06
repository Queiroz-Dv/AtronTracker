using Shared.Models;

namespace Shared.Interfaces
{
    public interface IMessages
    {
        public List<Message> Messages { get; }
    }
}