namespace Shared.Application.Interfaces.Mapping
{
    public abstract class Mapper<TEntity, TDto> : IMapper<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {
        public abstract TDto MapToDto(TEntity entity);

        public abstract TEntity MapToEntity(TDto dto);

        public abstract void MapToUpdate(TDto dto, TEntity entity);

        public IEnumerable<TDto> MapToDtos(IEnumerable<TEntity>? entities)
        {
            return entities.MapToDtos(this);
        }

        public IEnumerable<TEntity> MapToEntities(IEnumerable<TDto>? dtos)
        {
            return dtos.MapToEntities(this);
        }
    }
}
