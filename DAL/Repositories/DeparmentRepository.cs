using DAL.DAO;
using DAL.DTO;
using DAL.Generics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class DeparmentRepository : IGenericRepository<DEPARTMENT>
    {
        private Context _context = new Context();

        public DeparmentRepository(Context context)
        {
            _context = context;
        }

        public DeparmentRepository()
        {
        }

        public void CreateEntity(DEPARTMENT entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity), "The deparment does not be null.");
                }

                var department = new DEPARTMENT()
                {
                    ID = entity.ID,
                    DepartmentName = entity.DepartmentName
                };

                var context = _context;
                context._db.DEPARTMENTs.InsertOnSubmit(department);
                context._db.SubmitChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool ExistsDepartment(DEPARTMENT model)
        {
            throw new NotImplementedException();
        }

        public ICollection<DEPARTMENT> GetAllEntities()
        {
            var context = _context;
            var entities = context._db.DEPARTMENTs.ToList();
            return entities;
        }

        public DEPARTMENT GetEntityById(object id)
        {
            try
            {
                var context = _context;
                var deparmentId = id as DEPARTMENT;
                DEPARTMENT entityFind = context._db.DEPARTMENTs.FirstOrDefault(dpt => dpt.ID == deparmentId.ID);
                return entityFind;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void RemoveEntity(DEPARTMENT entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateEntity(DEPARTMENT entity)
        {
            throw new NotImplementedException();
        }
    }
}
