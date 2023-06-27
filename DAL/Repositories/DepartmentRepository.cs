using DAL.DAO;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DAL.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private Context _context = new Context();
        private DEPARTMENT department = new DEPARTMENT();

        public DepartmentRepository(Context context)
        {
            _context = context;
        }

        public DepartmentRepository()
        {
        }

        protected EmployeeDataClassDataContext GetContext()
        {
            var context = _context.GetContext();
            return context;
        }

        public void CreateEntityRepository(DEPARTMENT entity)
        {
            var db = GetContext();

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

                db.DEPARTMENTs.InsertOnSubmit(department);
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

        public IEnumerable<DEPARTMENT> GetAllEntitiesRepository()
        {
            try
            {
                var context = GetContext();
                var entities = context.DEPARTMENTs.ToList();
                return entities;

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error" + ex);
                throw;

            }
        }

        public DEPARTMENT GetEntityByIdRepository(object id)
        {
            try
            {
                var context = GetContext();
                var deparmentId = id as DEPARTMENT;
                DEPARTMENT entityFind = context.DEPARTMENTs.FirstOrDefault(dpt => dpt.ID == deparmentId.ID);
                return entityFind;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void RemoveEntityRepository(DEPARTMENT entity)
        {
            try
            {
                var context = GetContext();
                var department = context.DEPARTMENTs.First(depart => depart.ID == entity.ID);
                context.DEPARTMENTs.DeleteOnSubmit(department);
                context.SubmitChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DEPARTMENT UpdateEntityRepository(DEPARTMENT entity)
        {
            try
            {
                var context = GetContext();
                department = context.DEPARTMENTs.First(d => d.ID == entity.ID);
                department.DepartmentName = entity.DepartmentName;
                context.SubmitChanges();
                return department;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}