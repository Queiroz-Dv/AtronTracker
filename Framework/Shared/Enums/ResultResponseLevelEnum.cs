using System.ComponentModel;

namespace Shared.Enums
{
    [Serializable]
    public enum ResultResponseLevelEnum
    {
        [Description("Message")]
        Message,

        [Description("Error")]
        Error,

        [Description("Warning")]
        Warning
    }
}