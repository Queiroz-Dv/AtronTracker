using Entities;
using Factory.Interfaces;
using Models;
using Notification.Interfaces;
using Notification.Models;
using PersonalTracking.BLL.Interfaces;
using PersonalTracking.DAL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalTracking.BLL.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IModelFactory<Department, DepartmentDto> _modelFactory; //Factory do departamento
        public readonly INotificationService _notificationService; // Notification service 
        private readonly IDepartmentRepository _departmentRepository; // Repositório de departamento

        public DepartmentService(IDepartmentRepository departmentRepository, IModelFactory<Department, DepartmentDto> modelFactory, INotificationService notificationService)
        {
            // Injeta as dependências no construtor 
            _departmentRepository = departmentRepository;
            _modelFactory = modelFactory;
            _notificationService = notificationService;
        }

        public async Task<DepartmentDto> Create(DepartmentDto entity)
        {
            var departmentModel = _modelFactory.ToModel(entity); // Define os valores
            var department = _modelFactory.SetAndValidateModel(departmentModel); // Valida a model
            var notifications = GetFactoryMessages();

            if (!notifications.HasErrors()) // Verifica se tem erros na Factory
            {
                await _departmentRepository.CreateDepartment(department); // Cria a entidade no repositório

                if (_departmentRepository.GetNotifications().HasErrors()) // Verifica se tem erros na DAL
                {
                    FillMessages();
                    return entity;
                }
                else
                {
                    _notificationService.AddMessage($"Department {department.Name} saved suceffuly");
                }
            }

            _notificationService.Messages.AddRange(notifications);
            return entity;
        }


        public async Task<IEnumerable<DepartmentDto>> GetDepartments()
        {
            var departments = await _departmentRepository.GetDepartments(); // Obtém todas as entidades de departamento do repositório

            var departmentsDTO = new List<DepartmentDto>();
            foreach (var item in departments)
            {
                var dto = _modelFactory.ToDto(item);
                departmentsDTO.Add(dto);
            }

            return departmentsDTO; // Retorna a lista de entidades de departamento
        }

        public async Task<DepartmentDto> GetById(int? id)
        {
            var entity = await _departmentRepository.GetDepartmentById(id); // Obtém uma entidade de departamento pelo ID do repositório
            if (_departmentRepository.GetNotifications().HasErrors())
            {
                FillMessages();
                return null;
            }

            var department = _modelFactory.ToDto(entity);

            return department; // Retorna a entidade de departamento encontrada
        }

        public async Task<DepartmentDto> Remove(DepartmentDto dto)
        {
            var department = _modelFactory.ToModel(dto); // Define os valores
            var departmentRepository = await _departmentRepository.RemoveDepartment(department); // Remove a entidade do repositório

            var departmentDto = _modelFactory.ToDto(departmentRepository);

            return departmentDto;
        }

        public async Task<DepartmentDto> Update(DepartmentDto dto)
        {
            var department = _modelFactory.ToModel(dto); // Define os valores
            var departmentRepository = await _departmentRepository.UpdateDepartment(department); // Remove a entidade do repositório

            var departmentDto = _modelFactory.ToDto(departmentRepository);

            return departmentDto; // Retorna a entidade atualizada
        }

        private void FillMessages()
        {
            foreach (var item in _departmentRepository.GetNotifications()) // Obtém cada mensagem na DAL
            {
                _notificationService.Messages.Add(item); // Insere na lista de mensagens da BLL
            }
        }

        public List<NotificationMessage> GetNotifications()
        {
            return _notificationService.Messages;
        }

        private List<NotificationMessage> GetFactoryMessages()
        {
            return _modelFactory.Notifications;
        }
    }
}
