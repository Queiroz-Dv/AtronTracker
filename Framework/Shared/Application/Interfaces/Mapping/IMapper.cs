namespace Shared.Application.Interfaces.Mapping
{
    public interface IMapper<TEntity, TDto>
        : IToDtoMapper<TEntity, TDto>,
          IToEntityMapper<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {    }
}