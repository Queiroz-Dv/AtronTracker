using DAL.DAO;
using DAL.Generics;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class EmployeeRepository : IEmployeeRepository<EMPLOYEE>, IGenericRepository<EMPLOYEE>
    {
        private Context _context = new Context();
        private EMPLOYEE _employee = new EMPLOYEE();

        public EmployeeRepository(Context context)
        {
            _context = context;
        }

        public EmployeeRepository() { }

        public void CreateEntity(EMPLOYEE entity)
        {
            try
            {
                var context = _context;
                context._db.EMPLOYEEs.InsertOnSubmit(entity);
                context._db.SubmitChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<EMPLOYEE> GetAllEntities()
        {
            throw new NotImplementedException();
        }

        public EMPLOYEE GetEntityById(object id)
        {
            throw new NotImplementedException();
        }

        public void RemoveEntity(EMPLOYEE entity)
        {
            throw new NotImplementedException();
        }

        public EMPLOYEE UpdateEntity(EMPLOYEE entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EMPLOYEE> GetUsers(int user)
        {
            var context = _context;
            var userFinded = context._db.EMPLOYEEs.Where(emp => emp.UserNo == user).ToList();
            return userFinded;
        }
    }
}
