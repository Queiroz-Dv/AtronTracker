using System.Collections.Generic;

namespace PersonalTracking.BLL.Interfaces
{
    public interface IGenericServices<TEntity> where TEntity : class
    {
        TEntity GetEntityByIdService(object id);

        IEnumerable<TEntity> GetAllService();

        TEntity CreateEntityService(TEntity entity);

        TEntity UpdateEntityService(TEntity entity);

        TEntity RemoveEntityService(object entity);
    }
}