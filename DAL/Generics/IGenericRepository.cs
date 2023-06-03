using System.Collections.Generic;

namespace DAL.Generics
{
    public interface IGenericRepository<T> where T : class, IEntity
    {
        IEnumerable<T> GetAllEntitiesRepository(T entity);

        void CreateEntityRepository(T entity);

        T GetEntityByIdRepository(int id);

        void UpdateEntityRepository(T entity);

        void RemoveEntityRepository(T entity);
    }
}