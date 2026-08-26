namespace Shared.Application.Interfaces.Mapping
{
    public interface IUpdateMapper<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {
        void MapToUpdate(TDto dto, TEntity entity);
    }
}
