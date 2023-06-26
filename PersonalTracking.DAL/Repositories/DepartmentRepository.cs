using PersonalTracking.DAL.DAO;
using PersonalTracking.DAL.DataAcess;
using PersonalTracking.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalTracking.DAL.Repositories
{
    public class DepartmentRepository : IDepartmentRepository<DEPARTMENT>
    {
        private DEPARTMENT department = new();

        private PersonalTrackingDBDataContext GetContext()
        {
            var context = Context.DB;
            return context;
        }

        public void CreateEntityRepository(DEPARTMENT entity)
        {
            // Posso utilizar a interface de implementaçao de context
            var context = GetContext();

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


                context.DEPARTMENTs.InsertOnSubmit(department);
                context.SubmitChanges();
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

                //MessageBox.Show("Error" + ex);
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