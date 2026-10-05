using Shared.Domain.ValueObjects;
using System.Collections.Generic;

namespace Shared.Application.Interfaces.Service
{
    public interface IMessageBaseService
    {
        IReadOnlyCollection<NotificationMessage> Messages { get; }
    }
}
