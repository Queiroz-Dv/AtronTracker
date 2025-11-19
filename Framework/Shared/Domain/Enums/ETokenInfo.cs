using System.ComponentModel;

namespace Shared.Domain.Enums
{
    public enum ETokenInfo
    {
        [Description("AccessToken")]
        AcesssToken,

        [Description("RefreshToken")]
        RefreshToken,

        [Description("AccessTokenExpires")]
        AccessTokenExpires,
    }
}