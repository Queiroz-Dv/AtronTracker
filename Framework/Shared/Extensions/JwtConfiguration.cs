using Microsoft.Extensions.Configuration;
using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;
using Shared.Domain.Enums;
using System.Security.Claims;

namespace Shared.Extensions
{
    public static class JwtConfiguration
    {
        public static string GetSecretKey(this IConfiguration configuration)
        {
            return configuration[EnumJwt.SecretKey.GetDescription()];
        }

        public static string GetIssuer(this IConfiguration configuration)
        {
            return configuration[EnumJwt.Issuer.GetDescription()];
        }

        public static string GetAudience(this IConfiguration configuration)
        {
            return configuration[EnumJwt.Audience.GetDescription()];
        }

        public static Claim[] GetClaims(DadosDoUsuarioDTO dadosDoUsuario)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, dadosDoUsuario.NomeDoUsuario),
                new Claim(ClaimTypes.Email, dadosDoUsuario.Email),
                new Claim(ClaimCode.CODIGO_USUARIO, dadosDoUsuario.CodigoDoUsuario),
                new Claim(ClaimCode.CODIGO_CARGO, dadosDoUsuario.CodigoDoCargo.IsNullOrEmpty() ? "" : dadosDoUsuario.CodigoDoCargo),
                new Claim(ClaimCode.CODIGO_DEPARTAMENTO, dadosDoUsuario.CodigoDoDepartamento.IsNullOrEmpty() ? "" : dadosDoUsuario.CodigoDoDepartamento),
            };

            return claims.ToArray();
        }
    }
}