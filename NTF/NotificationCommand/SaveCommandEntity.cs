using BLL.NotificationCommand;
using NTF.Entities;
using NTF.ErrorsType;

namespace NTF.NotificationCommand
{
    public class SaveCommandEntity : Command
    {
        private readonly EntityModel notification;

        public SaveCommandEntity(object entity) : base(entity)
        {
            notification = entity as EntityModel;
            var description = new ErrorDescription("New record created!", new Warning());
            notification.Errors.Add(description);
        }

        public void RunCommand()
        {
            if (!Errors.HasErrors)
            {
                Recording();
            }
            else
            {
                var error = new ErrorDescription("The record was not registered", new Critical());
                notification.Errors.Add(error);
            }
        }

        private void Recording()
        {
            var message = new ErrorDescription("Record saved", new Information());
            notification.Errors.Add(message);
        }
    }
}
