using DAL.DAO;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DAL.Repositories
{
    public class DepartmentRepository : IDepartmentRepository<DEPARTMENT>
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

        public void CreateEntityRepository(DEPARTMENT entity)
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

        public IEnumerable<DEPARTMENT> GetAllEntitiesRepository()
        {
            try
            {
                var context = _context;
                var entities = context._db.DEPARTMENTs.ToList();
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

        public void RemoveEntityRepository(DEPARTMENT entity)
        {
            try
            {
                var context = _context;
                var department = context._db.DEPARTMENTs.First(depart => depart.ID == entity.ID);
                context._db.DEPARTMENTs.DeleteOnSubmit(department);
                context._db.SubmitChanges();
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
                var context = _context;
                department = context._db.DEPARTMENTs.First(d => d.ID == entity.ID);
                department.DepartmentName = entity.DepartmentName;
                context._db.SubmitChanges();
                return department;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}