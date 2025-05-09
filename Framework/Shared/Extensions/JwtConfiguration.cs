﻿using Microsoft.Extensions.Configuration;
using Shared.DTO.API;
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

        public static Claim[] GetClaims(DadosDoUsuario dadosDoUsuario)
        {
            var claims = new[] {
                new Claim(ClaimTypes.Name, dadosDoUsuario.NomeDoUsuario),
                new Claim(ClaimTypes.Email, dadosDoUsuario.Email),
                new Claim(ClaimTypes.Expiration, dadosDoUsuario.Expiracao.ToString()),
                new Claim(ClaimCode.CODIGO_USUARIO, dadosDoUsuario.CodigoDoUsuario),
                new Claim(ClaimCode.CODIGO_CARGO, dadosDoUsuario.CodigoDoCargo.IsNullOrEmpty() ? "" : dadosDoUsuario.CodigoDoCargo),
                new Claim(ClaimCode.CODIGO_DEPARTAMENTO, dadosDoUsuario.CodigoDoDepartamento.IsNullOrEmpty() ? "" : dadosDoUsuario.CodigoDoDepartamento),
                // TODO: Reestruturar a criação dos GUIDs para não serem gerados aqui
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Identificador único do token
            };

            return claims;
        }
    }
}