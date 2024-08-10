using System.ComponentModel;

namespace Shared.Enums
{
    public enum ResultResponseLevelEnum
    {
        [Description("Success")]
        Success,

        [Description("Error")]
        Error,

        [Description("Warning")]
        Warning
    }
}