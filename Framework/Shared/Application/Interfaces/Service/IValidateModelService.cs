using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface IValidateModelService<Entity>
    {
        public abstract void Validate(Entity entity);        
    }

    public interface IValidador<Entity>
    {
        IList<NotificationMessage> Validar(Entity entity);
    }
}