using Shared.Domain.Events;

namespace Shared.Application.Messaging;

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task ManipularAsync(TEvent @event, CancellationToken cancellationToken);
}
