using DAL.Factory;
using DAL.Interfaces;
using PersonalTracking.Entities;
using PersonalTracking.Factory.Interfaces;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DAL.Repositories
{
    public class DepartmentRepository : ContextBase<DepartmentModel, DEPARTMENT>, IDepartmentRepository
    {
        public DepartmentRepository(IModelFactory<DepartmentModel, DEPARTMENT> modelFactory) : base(modelFactory)
        {
            _factory = modelFactory;
        }

        public void CreateEntityRepository(DepartmentModel model)
        {
            var db = GetContext(); // Obtém o contexto do banco de dados

            try
            {
                // Converte o modelo de departamento em uma entidade DAL
                var entity = _factory.ToEntity(model);

                db.DEPARTMENTs.InsertOnSubmit(entity); // Insere a entidade no contexto
                db.SubmitChanges(); // Salva as alterações no banco de dados
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<DepartmentModel> GetAllEntitiesRepository()
        {
            try
            {
                var context = GetContext(); // Obtém o contexto do banco de dados
                var entities = context.DEPARTMENTs.ToList(); // Obtém todas as entidades de departamento

                var departments = new List<DepartmentModel>();

                foreach (var entity in entities)
                {
                    // Converte as entidades uma por uma em modelos de departamento
                    var department = _factory.ToModel(entity);

                    departments.Add(department);
                }

                // Retorna a lista de modelos de departamento
                return departments;

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
                var context = GetContext(); // Obtém o contexto do banco de dados
                var entity = context.DEPARTMENTs.FirstOrDefault(dpt => dpt.ID.Equals(id)); // Obtém a entidade de departamento pelo ID

                // Converte a entidade em um modelo de departamento
                //var departmentModel = _objectModelHelper.CreateModel(departmentEntity, department => _modelFactory.CreateDalToModel(departmentEntity));
                var department = _factory.ToModel(entity);

                return department; // Retorna o modelo de departamento encontrado
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DepartmentModel RemoveEntityRepository(object entity)
        {
            try
            {
                var context = GetContext(); // Obtém o contexto do banco de dados

                var entityDAL = context.DEPARTMENTs.FirstOrDefault(depart => depart.ID.Equals(entity)); // Obtém a entidade de departamento a ser removida

                context.DEPARTMENTs.DeleteOnSubmit(entityDAL); // Remove a entidade do contexto

                context.SubmitChanges(); // Salva as alterações no banco de dados

                var department = _factory.ToModel(entityDAL);   

                return department;
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
                var context = GetContext(); // Obtém o contexto do banco de dados
                var entityDAL = context.DEPARTMENTs.FirstOrDefault(depart => depart.ID.Equals(entity.DepartmentModelId));  // Obtém a entidade de departamento a ser atualizada

                entityDAL.DepartmentName = entity.DepartmentModelName; // Atualiza o nome do departamento na entidade

                context.SubmitChanges(); // Salva as alterações no banco de dados

                var department = _factory.ToModel(entityDAL);

                return department; // Retorna o modelo de departamento atualizado
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}