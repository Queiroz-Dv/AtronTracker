namespace Shared.Domain.Events;

public interface IEvent
{
    Guid EventId { get; }
    DateTime DataOcorrencia { get; }
}
