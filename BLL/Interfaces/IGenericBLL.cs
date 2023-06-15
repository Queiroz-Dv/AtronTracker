using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface IGenericBLL<TEntity> where TEntity : class/*, IEntity*/
    {
        TEntity GetEntityByIdBLL(object id);

        IEnumerable<TEntity> GetAllEntitiesBLL();

        TEntity CreateEntityBLL(TEntity entity);

        TEntity UpdateEntityBLL(TEntity entity);

        void RemoveEntityBLL(TEntity entity);
    }
}