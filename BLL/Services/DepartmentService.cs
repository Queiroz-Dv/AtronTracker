using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Interfaces.FactoryModules;
using PersonalTracking.Models;
using System.Collections.Generic;

namespace BLL.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository; // Repositório de departamento
        private readonly IDepartmentFactory _departmentFactory;

        public DepartmentService(IDepartmentRepository departmentRepository, IDepartmentFactory departmentFactory)
        {
            _departmentRepository = departmentRepository; // Injeta o repositório de departamento na classe
            _departmentFactory = departmentFactory;
        }

        public DepartmentModel CreateEntityService(DepartmentModel entity)
        {
            var departmentModel = _departmentFactory.SetDepartmentModelFactory(entity); // Define os valores do modelo de departamento

            if (departmentModel != null)
            {
                _departmentRepository.CreateEntityRepository(departmentModel); // Cria a entidade no repositório
            }
           
            return departmentModel; // Retorna o modelo de departamento criado
        }
 
        public IEnumerable<DepartmentModel> GetAllService()
        {
            var departments = _departmentRepository.GetAllEntitiesRepository(); // Obtém todas as entidades de departamento do repositório
            return departments; // Retorna a lista de entidades de departamento
        }

        public List<DepartmentModel> GetAllModelService()
        {
            var departments = _departmentRepository.GetAllDepartmentEntities(); // Obtém todos os modelos de departamento do repositório
            return departments as List<DepartmentModel>; // Retorna a lista de modelos de departamento
        }

        public DepartmentModel GetEntityByIdService(object id)
        {
            var entity = _departmentRepository.GetEntityByIdRepository(id); // Obtém uma entidade de departamento pelo ID do repositório
            return entity; // Retorna a entidade de departamento encontrada
        }

        public DepartmentModel RemoveEntityService(object model)
        {
            var repository = _departmentRepository.RemoveEntityRepository(model); // Remove a entidade do repositório
            return repository;
        }

        public DepartmentModel UpdateEntityService(DepartmentModel entity)
        {
            var departmentEntity = _departmentFactory.SetDepartmentModelFactory(entity); // Define os valores do modelo de departamento

            _departmentRepository.UpdateEntityRepository(departmentEntity); // Atualiza a entidade no repositório
            return entity; // Retorna a entidade atualizada
        }

        public DepartmentModel CreateDepartmentModelObjectFactory()
        {
            var newDepartmentModel = _departmentFactory.CreateDepartmentModelFactory();
            return newDepartmentModel;
        }
    }
}
