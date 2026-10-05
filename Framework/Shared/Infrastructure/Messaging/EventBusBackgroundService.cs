using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Application.Messaging;
using Shared.Domain.Events;

namespace Shared.Infrastructure.Messaging;

public class EventBusBackgroundService : BackgroundService
{
    private readonly Channel<IEvent> _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventBusBackgroundService> _logger;

    public EventBusBackgroundService(
        Channel<IEvent> channel,
        IServiceProvider serviceProvider,
        ILogger<EventBusBackgroundService> logger)
    {
        _channel = channel;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var @event in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                var eventType = @event.GetType();
                var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
                
                using var scope = _serviceProvider.CreateScope();
                var handlers = scope.ServiceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    if (handler is null) continue;

                    var method = handlerType.GetMethod(nameof(IEventHandler<IEvent>.ManipularAsync));
                    if (method is not null)
                    {
                        var task = (Task?)method.Invoke(handler, new object[] { @event, stoppingToken });
                        if (task is not null)
                        {
                            await task;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar evento {EventId}", @event.EventId);
            }
        }
    }
}