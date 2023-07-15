using DAL.DAO;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class EmployeeRepository : IEmployeeRepository<EMPLOYEE>
    {
        private Context _context;
        private EMPLOYEE _employee;

        public EmployeeRepository(Context context)
        {
            _context = context;
            _employee = new EMPLOYEE();
        }

        protected QRZDatabaseDataContext GetContext()
        {
            var context = _context.GetContext();
            return context;
        }

        public void CreateEntityRepository(EMPLOYEE entity)
        {
            try
            {
                var context = GetContext();
                context.EMPLOYEEs.InsertOnSubmit(entity);
                context.SubmitChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<EMPLOYEE> GetAllEntitiesRepository()
        {
            throw new NotImplementedException();
        }

        public EMPLOYEE GetEntityByIdRepository(object id)
        {
            var context = GetContext();
            var user = context.EMPLOYEEs.Where(employee => employee.UserNo.Equals(id)).FirstOrDefault();
            return user;
        }

        public EMPLOYEE RemoveEntityRepository(object entity)
        {
            throw new NotImplementedException();
        }

        public EMPLOYEE UpdateEntityRepository(EMPLOYEE entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EMPLOYEE> GetUsers(int user)
        {
            var context = GetContext();
            var userFinded = context.EMPLOYEEs.Where(emp => emp.UserNo == user).ToList();
            return userFinded;
        }

        public IEnumerable<EMPLOYEE> GetEmployeesByUserNoAndPassword(int userNumber, string password)
        {
            var context = GetContext();

            try
            {
                var entities = context.EMPLOYEEs.Where(x => x.UserNo == userNumber &&
                                                            x.Password == password).ToList();
                return entities;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}