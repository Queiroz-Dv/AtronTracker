using System.Collections.Generic;

namespace DAL.Generics
{
    public interface IGenericRepository<TEntity> where TEntity : class/*, IEntity*/
    {
        TEntity GetEntityById(object id);

        IEnumerable<TEntity> GetAllEntities();

        void CreateEntity(TEntity entity);

        TEntity UpdateEntity(TEntity entity);

        void RemoveEntity(TEntity entity);
    }
}