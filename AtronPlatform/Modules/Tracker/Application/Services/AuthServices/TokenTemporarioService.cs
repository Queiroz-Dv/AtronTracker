using Application.Interfaces.Services;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.AuthServices;

public sealed class TokenTemporarioService : ITokenTemporarioService
{
    private const int TamanhoTokenEmBytes = 32;

    public TokenTemporario Criar()
    {
        var bytes = RandomNumberGenerator.GetBytes(TamanhoTokenEmBytes);
        var valor = Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        return new TokenTemporario(valor, ObterHash(valor));
    }

    public string ObterHash(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return string.Empty;

        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
