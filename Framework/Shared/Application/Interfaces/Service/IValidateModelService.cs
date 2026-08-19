namespace Shared.Application.Interfaces.Service
{
    public interface IValidateModelService<Entity>
    {
        public abstract void Validate(Entity entity);
    }
}