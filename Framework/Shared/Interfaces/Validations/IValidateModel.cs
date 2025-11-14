using Shared.Models;

namespace Shared.Interfaces.Validations
{
    public interface IValidateModel<Entity>
    {
        public abstract void Validate(Entity entity);        
    }

    public interface IValidador<Entity>
    {
        IList<Message> Validar(Entity entity);
    }
}