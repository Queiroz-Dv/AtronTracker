﻿using System.Collections.Generic;

namespace PersonalTracking.DAL.Generics
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity GetEntityByIdRepository(object id);

        IEnumerable<TEntity> GetAllEntitiesRepository();

        void CreateEntityRepository(TEntity entity);

        TEntity UpdateEntityRepository(TEntity entity);

        void RemoveEntityRepository(TEntity entity);
    }
}