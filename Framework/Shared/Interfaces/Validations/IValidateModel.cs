namespace Shared.Interfaces.Validations
{
    public interface IValidateModel<Entity>
    {
        public abstract void Validate(Entity entity);
    }
}