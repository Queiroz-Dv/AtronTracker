namespace PersonalTracking.Helper.Interfaces
{
    public interface IValidateHelper<TEntity> 
    {
        void Validate(TEntity entity);
    }
}