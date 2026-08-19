using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface IValidador<Entity>
    {     
        IEnumerable<NotificationMessage> Validar(Entity entity);
    }
}