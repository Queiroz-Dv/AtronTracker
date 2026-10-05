using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Messaging;
using Shared.Domain.Events;
using Shared.Infrastructure.Messaging;

namespace Shared.Extensions;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryEventBus(this IServiceCollection services)
    {
        services.AddSingleton(Channel.CreateUnbounded<IEvent>(new UnboundedChannelOptions
        {
            SingleReader = true, // EventBusBackgroundService será o único leitor contínuo
            SingleWriter = false
        }));

        services.AddSingleton<IEventBus, InMemoryEventBus>();
        services.AddHostedService<EventBusBackgroundService>();

        return services;
    }
}
