using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Shared.Services
{
    public class ApplicationTokenService : IApplicationTokenService
    {
        private readonly IConfiguration _configuration;

        public ApplicationTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public UserToken GenerateToken(string usuarioNome, string codigoDeUsuario, string email)
        {
            // Objeto de configuração do token
            var appSecurityToken = new ApplicationSecurityToken()
            {
                SecretKey = _configuration.GetSecretKey(), // Chave secreta
                Issuer = _configuration.GetIssuer(),      // Emissor
                Audience = _configuration.GetAudience(),  // Audiência
                Claims = JwtConfiguration.GetClaims(usuarioNome, codigoDeUsuario, email) // Claims
            };

            // Secret Key como array de bytes
            var secretKeyEncoding = Encoding.UTF8.GetBytes(appSecurityToken.SecretKey);

            // Gerar chave privada e assinar o token
            var privateKey = new SymmetricSecurityKey(secretKeyEncoding);

            // Gerar a assinatura digital do token
            var userSigningCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            // Definir o tempo de expiração do token
            var tokenExpiration = DateTime.Now.AddHours(1);

            // Gerar o token
            var token = new JwtSecurityToken(
                issuer: appSecurityToken.Issuer,
                audience: appSecurityToken.Audience,
                claims: appSecurityToken.Claims,
                expires: tokenExpiration,
                signingCredentials: userSigningCredentials
            );

            try
            {
                var tokenCreated = new JwtSecurityTokenHandler().WriteToken(token);

                // Retornar o token
                var userToken = new UserToken()
                {
                    Token = tokenCreated,
                    Expires = tokenExpiration
                };

                return userToken;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}