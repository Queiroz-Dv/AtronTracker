using DAL.DAO;
using DAL.Factory;
using DAL.Interfaces;
using HLP.Entity;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DAL.Repositories
{
    public class DepartmentRepository : DepartmentFactory, IDepartmentRepository
    {
        public DepartmentRepository(Context context)
        {
            _context = context; // Define o contexto do banco de dados
        }

        public DepartmentRepository() { }

        public void CreateEntityRepository(DepartmentModel entity)
        {
            var db = GetContext(); // Obtém o contexto do banco de dados

            try
            {
                // Converte o modelo de departamento em uma entidade DAL
                var departmentDal = convertObject.ConvertObject(entity, departmentModel => CreateDepartmentDal(departmentModel));

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
                var departmentConverted = convertObject.ConvertList(entities, departmentsConverted => CreateDepartmentModel(departmentsConverted));

                // Retorna a lista de modelos de departamento
                return departmentConverted;

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
                var departmentId = id as DepartmentModel; // Obtém o ID do departamento a partir do parâmetro
                var departmentEntity = context.DEPARTMENTs.FirstOrDefault(dpt => dpt.ID.Equals(departmentId.DepartmentModelId)); // Obtém a entidade de departamento pelo ID

                // Converte a entidade em um modelo de departamento
                var departmentModel = convertObject.ConvertObject(departmentEntity, departmentObject => CreateDepartmentModel(departmentEntity));

                return departmentModel; // Retorna o modelo de departamento encontrado
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
                var context = GetContext(); // Obtém o contexto do banco de dados
                var department = context.DEPARTMENTs.FirstOrDefault(depart => depart.ID.Equals(entity.DepartmentModelId)); // Obtém a entidade de departamento a ser removida

                context.DEPARTMENTs.DeleteOnSubmit(department); // Remove a entidade do contexto
                context.SubmitChanges(); // Salva as alterações no banco de dados
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
                var departmentEntity = context.DEPARTMENTs.First(d => d.ID.Equals(entity.DepartmentModelId)); // Obtém a entidade de departamento a ser atualizada
                departmentEntity.DepartmentName = entity.DepartmentModelName; // Atualiza o nome do departamento na entidade
                context.SubmitChanges(); // Salva as alterações no banco de dados

                // Converte a entidade em um modelo de departamento
                var departmentModel = convertObject.ConvertObject(departmentEntity, department => CreateDepartmentModel(departmentEntity));
                return departmentModel; // Retorna o modelo de departamento atualizado
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
                var context = GetContext(); // Obtém o contexto do banco de dados
                var entities = context.DEPARTMENTs.ToList(); // Obtém todas as entidades de departamento

                // Converte as entidades em modelos de departamento
                List<DepartmentModel> departments = convertObject.ConvertList(entities, departmentModel => CreateDepartmentModel(departmentModel));

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