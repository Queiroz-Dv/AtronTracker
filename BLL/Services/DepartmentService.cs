using BLL.Interfaces;
using BLL.Validation;
using DAL.Interfaces;
using PersonalTracking.Entities;
using PersonalTracking.Factory.Interfaces;
using PersonalTracking.Models;
using PersonalTracking.Notification.Interfaces;
using PersonalTracking.Notification.Models;
using System.Collections.Generic;

namespace BLL.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IModelFactory<DepartmentModel, DEPARTMENT> _modelFactory; //Factory do departamento
        public readonly INotificationService _notificationService; // Notification service 
        private readonly IDepartmentRepository _departmentRepository; // Repositório de departamento

        public List<NotificationMessage> Messages { get; set; }

        public DepartmentService(IDepartmentRepository departmentRepository, IModelFactory<DepartmentModel, DEPARTMENT> modelFactory, INotificationService notificationService)
        {
            // Injeta as dependências no construtor 
            _departmentRepository = departmentRepository;
            _modelFactory = modelFactory;
            _notificationService = notificationService;
            Messages = new List<NotificationMessage>();
        }

        public DepartmentModel CreateEntityService(DepartmentModel entity)
        {
            var departmentModel = _modelFactory.SetAndValidateModel(entity); // Define os valores e valida o modelo

            var notifications = GetMessages();

            if (!notifications.HasErrors())
            {
                _departmentRepository.CreateEntityRepository(departmentModel); // Cria a entidade no repositório
                _notificationService.AddMessage("Department saved suceffuly");
                return departmentModel; // Retorna o modelo de departamento criado
            }

            _notificationService.Messages.AddRange(notifications);
            return entity;
        }

        private List<NotificationMessage> GetMessages()
        {
            return _modelFactory.Notifications;
        }

        public IEnumerable<DepartmentModel> GetAllService()
        {
            var departments = _departmentRepository.GetAllEntitiesRepository(); // Obtém todas as entidades de departamento do repositório
            return departments; // Retorna a lista de entidades de departamento
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
            var departmentEntity = _modelFactory.SetModelWhithoutValidation(entity); // Define os valores do modelo sem validação

            _departmentRepository.UpdateEntityRepository(departmentEntity); // Atualiza a entidade no repositório
            return entity; // Retorna a entidade atualizada
        }
    }
}
