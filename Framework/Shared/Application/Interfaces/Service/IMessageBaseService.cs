using Shared.Models;

namespace Shared.Application.Interfaces.Service
{
    public interface IMessageBaseService
    {
        public List<Message> Notificacoes { get; }
    }
}