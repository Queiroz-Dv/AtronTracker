using BLL.Interfaces;
using BLL.Validation;
using DAL.Interfaces;
using PersonalTracking.Notification.Models;
using DAL.Interfaces.FactoryModules;
using PersonalTracking.Models;
using System.Collections.Generic;

namespace BLL.Services
{
    public class DepartmentService : NotificationModel, IDepartmentService
    {
        private readonly DepartmentValidationService _validationService;
        private readonly IDepartmentRepository _departmentRepository; // Repositório de departamento
        private readonly IDepartmentFactory _departmentFactory;

        public DepartmentService(IDepartmentRepository departmentRepository, IDepartmentFactory departmentFactory, DepartmentValidationService validationService)
        {
            _departmentRepository = departmentRepository; // Injeta o repositório de departamento na classe
            _departmentFactory = departmentFactory;
            _validationService = validationService;
        }

        public DepartmentModel CreateEntityService(DepartmentModel entity)
        {
            var departmentModel = _departmentFactory.SetDepartmentModelFactory(entity); // Define os valores do modelo de departamento

            _validationService.Validate(entity);

            var notifications = GetMessages();

            Messages.AddRange(notifications);
            if (departmentModel != null)
            {
                if (!Messages.HasErrors())
                {
                    _departmentRepository.CreateEntityRepository(departmentModel); // Cria a entidade no repositório
                    AddMessage("Department saved suceffuly");
                    return departmentModel; // Retorna o modelo de departamento criado
                }
            }
            return entity;
        }

        private List<NotificationMessage> GetMessages()
        {
            return _validationService.notificationService.Messages;
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
