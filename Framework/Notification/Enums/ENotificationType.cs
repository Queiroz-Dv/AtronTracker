using System;
using System.ComponentModel;

namespace Notification.Enums
{
    /// <summary>
    /// Defines os níveis de notificação
    /// </summary>
    [Serializable]
    public enum ENotificationType
    {
        [Description("Error")]
        Error,

        [Description("Warning")]
        Warning,

        [Description("Message")]
        Message
    }
}