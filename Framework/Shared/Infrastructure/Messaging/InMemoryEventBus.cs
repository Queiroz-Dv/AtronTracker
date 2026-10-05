using System.Threading.Channels;
using Shared.Application.Messaging;
using Shared.Domain.Events;

namespace Shared.Infrastructure.Messaging;

public class InMemoryEventBus : IEventBus
{
    private readonly Channel<IEvent> _channel;

    public InMemoryEventBus(Channel<IEvent> channel)
    {
        _channel = channel;
    }

    public async ValueTask PublicarAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        await _channel.Writer.WriteAsync(@event, cancellationToken);
    }
}