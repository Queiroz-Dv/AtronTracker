using BLL.Interfaces;
using DAL.Interfaces;
using PersonalTracking.Models;
using System.Collections.Generic;

namespace BLL.Services
{
    public class DepartmentService : IDepartmentService
    {

        private readonly IDepartmentRepository _departmentRepository; // Repositório de departamento
        private DepartmentModel departmentModel; // Modelo de departamento

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository; // Injeta o repositório de departamento na classe
            departmentModel = new DepartmentModel(); // Inicializa o modelo de departamento com o objeto de mensagens
        }


        public DepartmentModel CreateEntityService(DepartmentModel entity)
        {
            var departmentModel = SetDepartmentModel(entity); // Define os valores do modelo de departamento

            _departmentRepository.CreateEntityRepository(departmentModel); // Cria a entidade no repositório

            return departmentModel; // Retorna o modelo de departamento criado
        }

        private DepartmentModel SetDepartmentModel(DepartmentModel _entity)
        {
            departmentModel.DepartmentModelId = _entity.DepartmentModelId; // Define o ID do modelo de departamento
            departmentModel.DepartmentModelName = _entity.DepartmentModelName; // Define o nome do modelo de departamento

            return departmentModel; // Retorna o modelo de departamento atualizado
        }

        public IEnumerable<DepartmentModel> GetAllService()
        {
            var departments = _departmentRepository.GetAllEntitiesRepository(); // Obtém todas as entidades de departamento do repositório
            return departments; // Retorna a lista de entidades de departamento
        }

        public List<DepartmentModel> GetAllModelService()
        {
            var departments = _departmentRepository.GetAllDepartmentEntities(); // Obtém todos os modelos de departamento do repositório
            return departments; // Retorna a lista de modelos de departamento
        }

        public DepartmentModel GetEntityByIdService(object id)
        {
            var entity = _departmentRepository.GetEntityByIdRepository(id); // Obtém uma entidade de departamento pelo ID do repositório
            return entity; // Retorna a entidade de departamento encontrada
        }

        public void RemoveEntityService(DepartmentModel model)
        {
            var _entity = SetDepartmentModel(model); // Define os valores do modelo de departamento

            _departmentRepository.RemoveEntityRepository(_entity); // Remove a entidade do repositório
        }

        public DepartmentModel UpdateEntityService(DepartmentModel entity)
        {
            var departmentEntity = SetDepartmentModel(entity); // Define os valores do modelo de departamento

            _departmentRepository.UpdateEntityRepository(departmentEntity); // Atualiza a entidade no repositório
            return entity; // Retorna a entidade atualizada
        }
    }
}
