namespace Shared.Application.Interfaces.Mapping
{
    public abstract class Mapper<TEntity, TDto>
        : Mapper<TEntity, TDto, TDto>, IMapper<TEntity, TDto>
        where TEntity : class
        where TDto : class
    { }

    public abstract class Mapper<TEntity, TInputDto, TOutputDto>
        : IToEntityMapper<TEntity, TInputDto>,
          IToDtoMapper<TEntity, TOutputDto>
        where TEntity : class
        where TInputDto : class
        where TOutputDto : class
    {
        public abstract TOutputDto MapToDto(TEntity entity);

        public abstract TEntity MapToEntity(TInputDto dto);

        public IEnumerable<TOutputDto> MapToDtos(IEnumerable<TEntity>? entities)
        {
            return entities.MapToDtos(this);
        }

        public IEnumerable<TEntity> MapToEntities(IEnumerable<TInputDto>? dtos)
        {
            return dtos.MapToEntities(this);
        }
    }
}
