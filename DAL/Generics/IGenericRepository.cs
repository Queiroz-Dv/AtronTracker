using System.Collections.Generic;

namespace DAL.Generics
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity GetEntityByIdRepository(object id);

        IEnumerable<TEntity> GetAllEntitiesRepository();

        void CreateEntityRepository(TEntity entity);

        TEntity UpdateEntityRepository(TEntity entity);

        TEntity RemoveEntityRepository(object entity);
    }
}