using Microsoft.Extensions.Configuration;
using Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shared.Extensions
{
    public static class JwtConfiguration
    {
        public static string GetSecretKey(this IConfiguration configuration)
        {
            return configuration[EnumJwt.SecretKey.GetEnumDescription()];
        }

        public static string GetIssuer(this IConfiguration configuration)
        {
            return configuration[EnumJwt.Issuer.GetEnumDescription()];
        }

        public static string GetAudience(this IConfiguration configuration)
        {
            return configuration[EnumJwt.Audience.GetEnumDescription()];
        }

        public static Claim[] GetClaims(string usuarioNome, string codigoDeUsuario, string email)
        {
            var claims = new[] {
                new Claim(ClaimTypes.Name, usuarioNome),                             // Nome do usuário
                new Claim("Codigo", codigoDeUsuario),                               // Código do usuário
                new Claim(ClaimTypes.Email, email),                                // E-mail do usuário
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Identificador único do token
            };

            return claims;
        }
    }
}