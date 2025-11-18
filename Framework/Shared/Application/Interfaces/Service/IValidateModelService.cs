using Shared.Models;

namespace Shared.Application.Interfaces.Service
{
    public interface IValidateModelService<Entity>
    {
        public abstract void Validate(Entity entity);        
    }

    public interface IValidador<Entity>
    {
        IList<Message> Validar(Entity entity);
    }
}