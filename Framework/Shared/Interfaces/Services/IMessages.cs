using Shared.Models;

namespace Shared.Interfaces.Services
{
    public interface IMessages
    {
        public List<Message> Notificacoes { get; }
    }
}