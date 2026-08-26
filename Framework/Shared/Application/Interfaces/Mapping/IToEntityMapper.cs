namespace Shared.Application.Interfaces.Mapping
{
    public interface IToEntityMapper<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {
        TEntity MapToEntity(TDto dto);

        IEnumerable<TEntity> MapToEntities(IEnumerable<TDto>? dtos);
    }
}
