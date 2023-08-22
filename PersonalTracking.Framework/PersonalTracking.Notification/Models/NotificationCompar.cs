using System.Collections.Generic;

namespace PersonalTracking.Notification.Models
{
    internal class NotificationCompar : IEqualityComparer<NotificationMessage>
    {
        public bool Equals(NotificationMessage x, NotificationMessage y)
        {
            return x.Message.Equals(y.Message) && x.Type.Equals(y.Type);
        }

        public int GetHashCode(NotificationMessage obj)
        {
            return (obj.Message + obj.Type).GetHashCode();
        }
    }
}