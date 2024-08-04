using System.ComponentModel;

namespace Shared.Enums
{
    public enum ResultResponseEnum
    {
        [Description("Success")]
        Success,

        [Description("Error")]
        Error,

        [Description("Warning")]
        Warning
    }
}