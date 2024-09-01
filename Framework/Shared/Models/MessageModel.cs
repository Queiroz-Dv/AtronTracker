using Shared.Enums;
using Shared.Services;

namespace Shared.Models
{
    [Serializable]
    public abstract class MessageModel<Entity> : MessageService
    {
        public override void AddError(string description)
        {
            AddNotification(description, MessageLevel.Error);
        }

        public override void AddMessage(string description)
        {
            AddNotification(description, MessageLevel.Message);
        }

        public override void AddWarning(string description)
        {
            AddNotification(description, MessageLevel.Warning);
        }

        public abstract void Validate(Entity entity);
    }
}