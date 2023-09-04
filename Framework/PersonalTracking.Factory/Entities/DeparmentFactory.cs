using PersonalTracking.Entities;
using PersonalTracking.Factory.Interfaces;
using PersonalTracking.Helper.Interfaces;
using PersonalTracking.Models;
using PersonalTracking.Notification.Models;
using System.Collections.Generic;

namespace PersonalTracking.Factory.Entities
{
    /// <summary>
    /// This factory will create, set and validated all objects of department 
    /// </summary>
    public class DeparmentFactory : IModelFactory<DepartmentModel, DEPARTMENT>
    {
        protected IValidateHelper<DepartmentModel> _validateHelper;

        public List<NotificationMessage> Notifications { get; set; }

        public DeparmentFactory(IValidateHelper<DepartmentModel> validateHelper)
        {
            _validateHelper = validateHelper;
            Notifications = new List<NotificationMessage>();
        }

        public DepartmentModel SetAndValidateModel(DepartmentModel model)
        {
            _validateHelper.Validate(model);

            var messages = GetNotifications();

            if (model != null)
            {
                if (!messages.HasErrors())
                {
                    var department = new DepartmentModel();
                    department.DepartmentModelId = model.DepartmentModelId;
                    department.DepartmentModelName = model.DepartmentModelName;

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

        public DEPARTMENT ToEntity(DepartmentModel model)
        {
            var modelSetted = SetAndValidateModel(model);

            if (!Notifications.HasErrors())
            {
                var entity = new DEPARTMENT()
                {
                    ID = modelSetted.DepartmentModelId,
                    DepartmentName = modelSetted.DepartmentModelName

                };

                return entity;
            }
            else
            {
                return null;
            }
        }

        public DepartmentModel ToModel(DEPARTMENT entity)
        {
            var department = new DepartmentModel()
            {
                DepartmentModelId = entity.ID,
                DepartmentModelName = entity.DepartmentName
            };

            return department;
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

        public DepartmentModel SetModelWhithoutValidation(DepartmentModel model)
        {
            var department = new DepartmentModel();
            department = model;
            return department;
        }
    }
}