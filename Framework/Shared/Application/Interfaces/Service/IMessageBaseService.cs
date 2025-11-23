using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface IMessageBaseService
    {
        public List<NotificationMessage> Notificacoes { get; }
    }
}