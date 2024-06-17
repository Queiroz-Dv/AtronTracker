using Entities;
using Factory.Interfaces;
using Helpers.Interfaces;
using Models;
using Notification.Models;
using System.Collections.Generic;

namespace Factory.Entities
{
    /// <summary>
    /// This factory will create, set and validated all objects of department 
    /// </summary>
    public class DeparmentFactory : IModelFactory<Department, DepartmentDto>
    {
        protected IValidateHelper<Department> _validateHelper;

        public List<NotificationMessage> Notifications { get; set; }

        public DeparmentFactory(IValidateHelper<Department> validateHelper)
        {
            _validateHelper = validateHelper;
            Notifications = new List<NotificationMessage>();
        }

        public Department SetAndValidateModel(Department model)
        {
            _validateHelper.Validate(model);

            var messages = GetNotifications();

            if (model != null)
            {
                if (!messages.HasErrors())
                {
                    var department = new Department();
                    department.Id = model.Id;
                    department.Name = model.Name;

                    return department;
                }

                return model;
            }
            else
            {
                if (messages.Count > decimal.Zero)
                {
                    messages.RemoveDuplicateMessages();

                    Notifications.AddRange(messages);
                }
            }

            return model;
        }

        /// <summary>
        /// Method to get notifications from notification service
        /// </summary>
        /// <returns>A list of notification message</returns>
        private List<NotificationMessage> GetNotifications()
        {
            var notifications = _validateHelper.NotificationService.Messages;

            Notifications.AddRange(notifications);

            return Notifications;
        }

        public Department SetModelWhithoutValidation(Department model)
        {
            Department department = model;
            return department;
        }

        public DepartmentDto ToDto(Department model)
        {
            var modelSetted = SetAndValidateModel(model);

            if (!Notifications.HasErrors())
            {
                var entity = new DepartmentDto()
                {
                    Id = modelSetted.Id,
                    Name = modelSetted.Name

                };

                return entity;
            }
            else
            {
                return null;
            }
        }

        public Department ToModel(DepartmentDto entity)
        {
            var department = new Department()
            {
                Id = entity.Id,
                Name = entity.Name
            };

            return department;
        }
    }
}