using System.Collections.Generic;

namespace DAL.Generics
{
    public interface IGenericRepository<TEntity> where TEntity : class/*, IEntity*/
    {
        TEntity GetEntityById(object id);

        ICollection<TEntity> GetAllEntities();

        void CreateEntity(TEntity entity);

        void UpdateEntity(TEntity entity);

        void RemoveEntity(TEntity entity);
    }
}