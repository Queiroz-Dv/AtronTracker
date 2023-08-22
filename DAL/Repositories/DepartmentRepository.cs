using DAL.Factory;
using DAL.Interfaces;
using DAL.Interfaces.FactoryModules;
using PersonalTracking.Helper.Interfaces;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DAL.Repositories
{
    public class DepartmentRepository : ContextBase<DepartmentModel, DEPARTMENT>, IDepartmentRepository
    {
        private readonly IDepartmentFactory _deparmentFactory;

        public DepartmentRepository(IDepartmentFactory departmentFactory, IObjectModelHelper<DepartmentModel, DEPARTMENT> objectModelHelper)
            : base(objectModelHelper)
        {
            _deparmentFactory = departmentFactory;
        }

        public void CreateEntityRepository(DepartmentModel model)
        {
            var db = GetContext(); // Obtém o contexto do banco de dados

            try
            {
                // Converte o modelo de departamento em uma entidade DAL
                var departmentDal = _objectModelHelper.CreateEntity(model, entity => _deparmentFactory.CreateModelToDalEntity(entity));

                db.DEPARTMENTs.InsertOnSubmit(departmentDal); // Insere a entidade no contexto
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

                // Converte as entidades em modelos de departamento
                var departments = _objectModelHelper.CreateListModels(entities, entitiesList => _deparmentFactory.CreateDalToModel(entitiesList));

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
                var departmentEntity = context.DEPARTMENTs.FirstOrDefault(dpt => dpt.ID.Equals(id)); // Obtém a entidade de departamento pelo ID

                // Converte a entidade em um modelo de departamento
                var departmentModel = _objectModelHelper.CreateModel(departmentEntity, department => _deparmentFactory.CreateDalToModel(departmentEntity));

                return departmentModel; // Retorna o modelo de departamento encontrado
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

                var department = context.DEPARTMENTs.FirstOrDefault(depart => depart.ID.Equals(entity)); // Obtém a entidade de departamento a ser removida

                context.DEPARTMENTs.DeleteOnSubmit(department); // Remove a entidade do contexto

                context.SubmitChanges(); // Salva as alterações no banco de dados

                var departmentModelToReturn = _objectModelHelper.CreateModel(department, departmentEntity => _deparmentFactory.CreateDalToModel(department));

                return departmentModelToReturn;
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
                var departmentEntity = context.DEPARTMENTs.FirstOrDefault(depart => depart.ID.Equals(entity.DepartmentModelId));  // Obtém a entidade de departamento a ser atualizada

                departmentEntity.DepartmentName = entity.DepartmentModelName; // Atualiza o nome do departamento na entidade

                context.SubmitChanges(); // Salva as alterações no banco de dados

                var departmentModelToReturn = _objectModelHelper.CreateModel(departmentEntity, department => _deparmentFactory.CreateDalToModel(departmentEntity));

                return departmentModelToReturn; // Retorna o modelo de departamento atualizado
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<DepartmentModel> GetAllDepartmentEntities()
        {
            try
            {
                var context = GetContext(); // Obtém o contexto do banco de dados
                var entities = context.DEPARTMENTs.ToList(); // Obtém todas as entidades de departamento

                // Converte as entidades em modelos de departamento
                var departments = _objectModelHelper.CreateListModels(entities, departmentModel => _deparmentFactory.CreateDalToModel(departmentModel));

                return departments; // Retorna a lista de modelos de departamento

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex);
                throw;
            }
        }
    }
}