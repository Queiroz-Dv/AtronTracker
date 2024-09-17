using System.ComponentModel;

namespace Shared.Enums
{
    [Serializable]
    public enum MessageLevel
    {
        [Description("Message")]
        Message,

        [Description("Error")]
        Error,

        [Description("Warning")]
        Warning
    }
}