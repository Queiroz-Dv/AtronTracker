using PersonalTracking.Entities;
using PersonalTracking.Factory.Interfaces;
using PersonalTracking.Helper.Interfaces;
using PersonalTracking.Models;
using PersonalTracking.Notification.Models;
using System;
using System.Collections.Generic;

namespace PersonalTracking.Factory.Entities
{
    public class PositionFactory : IModelFactory<PositionModel, POSITION>
    {
        protected IValidateHelper<PositionModel> _validateHelper;

        public List<NotificationMessage> Notifications { get; set; }

        public PositionFactory(IValidateHelper<PositionModel> validateHelper)
        {
            _validateHelper = validateHelper;
            Notifications = new List<NotificationMessage>();
        }

        private List<NotificationMessage> GetNotifications()
        {
            var notifications = _validateHelper.NotificationService.Messages;

            Notifications.AddRange(notifications);

            return Notifications;
        }

        public POSITION ToEntity(PositionModel model)
        {
            var modelSetted = SetAndValidateModel(model);

            if (!Notifications.HasErrors())
            {
                var entity = new POSITION()
                {
                    ID = modelSetted.PositionId,
                    PositionName = modelSetted.PositionName

                };

                return entity;
            }
            else
            {
                return null;
            }
        }

        public PositionModel ToModel(POSITION entity)
        {
            throw new NotImplementedException();
        }

        public PositionModel SetAndValidateModel(PositionModel model)
        {
            _validateHelper.Validate(model);

            var messages = GetNotifications();

            if (model != null)
            {
                if (!messages.HasErrors())
                {
                    var position = new PositionModel();
                    position.PositionId = model.PositionId;
                    position.PositionName = model.PositionName;

                    return position;
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

        public PositionModel SetModelWhithoutValidation(PositionModel model)
        {
            var position = new PositionModel();
            position = model;
            return position;
        }
    }
}
