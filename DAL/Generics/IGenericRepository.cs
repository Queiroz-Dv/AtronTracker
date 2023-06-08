using System.Collections.Generic;

namespace DAL.Generics
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAllEntitiesRepository(T entity);

        void CreateEntityRepository(T entity);

        T GetEntityByIdRepository(object id);

        void UpdateEntityRepository(T entity);

        void RemoveEntityRepository(T entity);
    }
}