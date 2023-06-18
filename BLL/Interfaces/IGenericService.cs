using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface IGenericServices<TEntity> where TEntity : class
    {
        TEntity GetEntityByIdService(object id);

        IEnumerable<TEntity> GetAllService();

        TEntity CreateEntityService(TEntity entity);

        TEntity UpdateEntityService(TEntity entity);

        void RemoveEntityService(TEntity entity);
    }
}