using DAL.DAO;
using DAL.Factory;
using DAL.Interfaces;
using DAL.Interfaces.FactoryModules;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DAL.Repositories
{
    public class DepartmentRepository : ContextBase<DepartmentModel, DEPARTMENT>, IDepartmentRepository
    {
        private readonly IDepartmentFactory DeparmentFactory;

        public DepartmentRepository()
        {
            _context = new Context(); // Define o contexto do banco de dados
            DeparmentFactory = new DepartmentFactory();
        }

        public void CreateEntityRepository(DepartmentModel model)
        {
            var db = GetContext(); // Obtém o contexto do banco de dados

            try
            {
                // Converte o modelo de departamento em uma entidade DAL
                //var departmentDal = convertObject.ConvertObject(entity, departmentModel => CreateDepartmentDal(departmentModel));
                var departmentDal = ObjectModelHelper.CreateEntity(model, entity => DeparmentFactory.CreateDepartmentDal(entity));

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
                //var departmentConverted = convertObject.ConvertList(entities, departmentsConverted => CreateDepartmentModel(departmentsConverted));
                var departments = ObjectModelHelper.CreateListModels(entities, entitiesList => DeparmentFactory.CreateDepartmentModel(entitiesList));

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
                //var departmentModel = convertObject.ConvertObject(departmentEntity, departmentObject => CreateDepartmentModel(departmentEntity));
                var departmentModel = ObjectModelHelper.CreateModel(departmentEntity, department => DeparmentFactory.CreateDepartmentModel(departmentEntity));

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
                var departmentObject = new DepartmentModel();

                departmentObject.DepartmentModelId = (int)entity;

                var context = GetContext(); // Obtém o contexto do banco de dados

                var departmentModel = GetEntityByIdRepository(departmentObject);

                var department = ObjectModelHelper.CreateEntity(departmentModel, departmentDal => DeparmentFactory.CreateDepartmentDal(departmentDal));

                context.DEPARTMENTs.DeleteOnSubmit(department); // Remove a entidade do contexto
                context.SubmitChanges(); // Salva as alterações no banco de dados

                var departmentModelToReturn = ObjectModelHelper.CreateModel(department, departmentEntity => DeparmentFactory.CreateDepartmentModel(department));

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
                var departmentEntity = GetEntityByIdRepository(entity.DepartmentModelId); // Obtém a entidade de departamento a ser atualizada

                departmentEntity.DepartmentModelName = entity.DepartmentModelName; // Atualiza o nome do departamento na entidade

                context.SubmitChanges(); // Salva as alterações no banco de dados

                return departmentEntity; // Retorna o modelo de departamento atualizado
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
                //List<DepartmentModel> departments = convertObject.ConvertList(entities, departmentModel => CreateDepartmentModel(departmentModel));
                var departments = ObjectModelHelper.CreateListModels(entities, departmentModel => DeparmentFactory.CreateDepartmentModel(departmentModel));

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