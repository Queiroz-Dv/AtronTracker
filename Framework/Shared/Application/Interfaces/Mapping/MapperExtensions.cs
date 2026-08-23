namespace Shared.Application.Interfaces.Mapping
{
    public static class MapperExtensions
    {
        public static TDto? MapToDto<TEntity, TDto>(
            this TEntity? entity,
            IToDtoMapper<TEntity, TDto> mapper)
            where TEntity : class
            where TDto : class
        {
            ArgumentNullException.ThrowIfNull(mapper);

            return entity is null
                ? null
                : mapper.MapToDto(entity);
        }

        public static TEntity? MapToEntity<TEntity, TDto>(
            this TDto? dto,
            IToEntityMapper<TEntity, TDto> mapper)
            where TEntity : class
            where TDto : class
        {
            ArgumentNullException.ThrowIfNull(mapper);

            return dto is null
                ? null
                : mapper.MapToEntity(dto);
        }

        public static IEnumerable<TDto> MapToDtos<TEntity, TDto>(
            this IEnumerable<TEntity>? entities,
            IToDtoMapper<TEntity, TDto> mapper)
            where TEntity : class
            where TDto : class
        {
            ArgumentNullException.ThrowIfNull(mapper);

            if (entities is null)
                return [];

            return entities.Select(mapper.MapToDto);
        }

        public static void MapToUpdate<TEntity, TDto>(
            this TEntity entity,
            TDto dto,
            IUpdateMapper<TEntity, TDto> mapper)
            where TEntity : class
            where TDto : class
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(mapper);

            mapper.MapToUpdate(dto, entity);
        }

        public static IEnumerable<TEntity> MapToEntities<TEntity, TDto>(
            this IEnumerable<TDto>? dtos,
            IToEntityMapper<TEntity, TDto> mapper)
            where TEntity : class
            where TDto : class
        {
            ArgumentNullException.ThrowIfNull(mapper);

            if (dtos is null)
                return [];

            return dtos.Select(mapper.MapToEntity);
        }
    }
}
