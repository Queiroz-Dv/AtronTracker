using Shared.Domain.Events;

namespace Shared.Application.Messaging;

public interface IEventBus
{
    ValueTask PublicarAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
}
