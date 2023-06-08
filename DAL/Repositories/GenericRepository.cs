using DAL.Generics;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        private readonly Table<T> _table;

        public GenericRepository(EmployeeDataClassDataContext context)
        {
            _table = context.GetTable<T>();
        }

        public void CreateEntityRepository(T entity)
        {
            _table.InsertOnSubmit(entity);
        }

        public IEnumerable<T> GetAllEntitiesRepository(T entity)
        {
            return _table;
        }

        public T GetEntityByIdRepository(int id)
        {
            var entity = _table.FirstOrDefault(x => x.Id == id);
            return entity;
        }

        public void RemoveEntityRepository(T entity)
        {
            _table.DeleteOnSubmit(entity);
        }

        public void UpdateEntityRepository(T entity)
        {
            // Assume que a classe T tem uma propriedade chamada "Id" que representa a chave primária
            T existingModel = _table.SingleOrDefault(x => x.Id == entity.Id);
            if (existingModel != null)
            {
                // Atualiza as propriedades do modelo existente com os valores do novo modelo
                // Aqui você pode usar uma biblioteca de mapeamento (como AutoMapper) ou fazer manualmente
                // Por exemplo: existingModel.Property1 = model.Property1;
                //             existingModel.Property2 = model.Property2;
            }
        }
    }
}
