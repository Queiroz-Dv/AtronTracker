using System.ComponentModel;

namespace Shared.Enums
{
    [Serializable]
    public enum EnumJwt
    {
        [Description("Jwt:SecretKey")]
        SecretKey,

        [Description("Jwt:Issuer")]
        Issuer,

        [Description("Jwt:Audience")]
        Audience,
    }
}