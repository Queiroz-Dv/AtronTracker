using DAL.DAO;
using DAL.Interfaces;
using System;
using System.Collections.Generic;

namespace DAL.Repositories
{
    public class DeparmentRepository : IDeparmentRepository
    {
        private readonly Context _context;
        private readonly EmployeeDataClassDataContext db;

        public DeparmentRepository(Context context)
        {
            _context = context;
        }

        public void CreateEntityRepository(DEPARTMENT entity)
        {
            try
            {
                _context.Departments.InsertOnSubmit(entity);
                db.SubmitChanges();
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

        public IEnumerable<DEPARTMENT> GetAllEntitiesRepository(DEPARTMENT entity)
        {
            throw new NotImplementedException();
        }

        public DEPARTMENT GetEntityByIdRepository(int id)
        {
            throw new NotImplementedException();
        }

        public void RemoveEntityRepository(DEPARTMENT entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateEntityRepository(DEPARTMENT entity)
        {
            throw new NotImplementedException();
        }
    }
}
