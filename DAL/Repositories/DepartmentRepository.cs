using DAL.DAO;
using DAL.Interfaces;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DAL.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private Context _context = new Context();
        private DEPARTMENT department;

        public DepartmentRepository(Context context)
        {
            _context = context;
            department = new DEPARTMENT();
        }

        public DepartmentRepository() { }

        protected EmployeeDataClassDataContext GetContext()
        {
            var context = _context.GetContext();
            return context;
        }

        public void CreateEntityRepository(DepartmentModel entity)
        {
            var db = GetContext();

            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity), "The deparment does not be null.");
                }

                //var department = new DEPARTMENT()
                //{
                //    ID = entity.ID,
                //    DepartmentName = entity.DepartmentName
                //};
                department.ID = entity.DepartmentModelId;
                department.DepartmentName = entity.DepartmentModelName;

                db.DEPARTMENTs.InsertOnSubmit(department);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool ExistsDepartment(DepartmentModel model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DepartmentModel> GetAllEntitiesRepository()
        {
            try
            {
                // TODO: Converter os objetos 
                var context = GetContext();
                var entities = context.DEPARTMENTs.ToList();
                return entities as DepartmentModel;

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error" + ex);
                throw;

            }
        }

        public DepartmentModel GetEntityByIdRepository(object id)
        {
            try
            {
                // TODO: Verificar e fazer a conversão
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

        public void RemoveEntityRepository(DepartmentModel entity)
        {
            try
            {
                // Verificar e converter 
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

        public DepartmentModel UpdateEntityRepository(DepartmentModel entity)
        {
            try
            {
                // TODO: Converter nesse ponto também
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

        public List<DepartmentModel> GetAllDepartmentEntities()
        {
            try
            {
                // TODO: Converter 
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
    }
}