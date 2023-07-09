using DAL.Interfaces;
using System;
using System.Collections.Generic;

namespace DAL.Repositories
{
    public class PositionRepository : IPositionRepository<POSITION>
    {
        public void CreateEntityRepository(POSITION entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<POSITION> GetAllEntitiesRepository()
        {
            throw new NotImplementedException();
        }

        public POSITION GetEntityByIdRepository(object id)
        {
            throw new NotImplementedException();
        }

        public POSITION RemoveEntityRepository(object entity)
        {
            throw new NotImplementedException();
        }

        public POSITION UpdateEntityRepository(POSITION entity)
        {
            throw new NotImplementedException();
        }
    }
}
