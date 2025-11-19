using System.ComponentModel;

namespace Shared.Domain.Enums
{
    public enum EHeaderInfo
    {
        [Description("XUSRCD")]
        CodigoDoUsuarioHeader,
        
        [Description("XUSRRTK")]
        RefreshTokenHeader,
    }
}