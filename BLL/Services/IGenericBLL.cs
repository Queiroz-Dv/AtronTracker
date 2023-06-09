using System.Collections.Generic;

namespace BLL.Services
{
    public interface IGenericBLL<TEntity> where TEntity : class/*, IEntity*/
    {
        TEntity GetEntityByIdBLL(object id);

        ICollection<TEntity> GetAllEntitiesBLL();

        TEntity CreateEntityBLL(TEntity entity);

        TEntity UpdateEntityBLL(TEntity entity);

        void RemoveEntityBLL(TEntity entity);
    }
}