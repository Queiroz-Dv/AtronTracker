namespace Shared.Application.Interfaces.Mapping
{
    public interface IToDtoMapper<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {
        TDto MapToDto(TEntity entity);

        IEnumerable<TDto> MapToDtos(IEnumerable<TEntity>? entities);
    }
}