using System.ComponentModel;

namespace Shared.Enums
{
    public enum EHeaderInfo
    {
        [Description("XUSRCD")]
        CodigoDoUsuarioHeader,
        
        [Description("XUSRRTK")]
        RefreshTokenHeader,
    }
}