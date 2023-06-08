using DAL.DAO;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class DeparmentRepository : IDeparmentRepository
    {
        private Context _context = new Context();
        //private readonly EmployeeDataClassDataContext db;

        public DeparmentRepository(Context context)
        {
            _context = context;
        }

        public DeparmentRepository()
        {
        }

        public void CreateEntityRepository(DEPARTMENT entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity), "O departamento não pode ser nulo.");
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

        public IEnumerable<DEPARTMENT> GetAllEntitiesRepository(DEPARTMENT entity)
        {
            throw new NotImplementedException();
        }

        public DEPARTMENT GetEntityByIdRepository(int id)
        {
            try
            {
                var context = _context;
                DEPARTMENT entityFind = context.Departments.FirstOrDefault(dpt => dpt.ID == id);
                return entityFind;
            }
            catch (Exception ex)
            {

                throw ex;
            }
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
