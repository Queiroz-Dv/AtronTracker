using System.Security.Cryptography;
using System.Text;

namespace Shared.Application.Security;

public static class RefreshTokenHash
{
    public static string Obter(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return string.Empty;

        return Convert.ToHexStringLower(
            SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }
}
