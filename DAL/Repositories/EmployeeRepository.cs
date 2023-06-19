﻿using DAL.DAO;
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

        public void CreateEntityRepository(EMPLOYEE entity)
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

        public IEnumerable<EMPLOYEE> GetAllEntitiesRepository()
        {
            throw new NotImplementedException();
        }

        public EMPLOYEE GetEntityByIdRepository(object id)
        {
            var context = _context.GetContext();
            var user = context.EMPLOYEEs.Where(employee => employee.UserNo.Equals(id)).FirstOrDefault();
            return user;
        }

        public void RemoveEntityRepository(EMPLOYEE entity)
        {
            throw new NotImplementedException();
        }

        public EMPLOYEE UpdateEntityRepository(EMPLOYEE entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EMPLOYEE> GetUsers(int user)
        {
            var context = _context;
            var userFinded = context._db.EMPLOYEEs.Where(emp => emp.UserNo == user).ToList();
            return userFinded;
        }

        public IEnumerable<EMPLOYEE> GetEmployeesByUserNoAndPassword(int userNumber, string password)
        {
            var context = _context;

            try
            {
                var entities = context._db.EMPLOYEEs.Where(x => x.UserNo == userNumber &&
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