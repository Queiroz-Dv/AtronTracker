using Atron.Domain.Interfaces.ApplicationInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces.Handlers;
using Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Services.Handlers
{
    public class TokenBuilderService : ITokenBuilderService
    {
        private readonly ILoginApplicationRepository _loginRepository;
        private readonly IConfiguration _configuration;
        private readonly MessageModel _messageModel;

        public TokenBuilderService(
            ILoginApplicationRepository loginRepository,
            IConfiguration configuration,
            MessageModel messageModel)
        {
            _loginRepository = loginRepository;
            _configuration = configuration;
            _messageModel = messageModel;
        }

        public string ObterCodigoUsuarioPorClaim(string token)
        {
            var codigoUsuario = ParseToken(token).Claims.FirstOrDefault(c => c.Type == ClaimCode.CODIGO_USUARIO)?.Value;

            if (codigoUsuario is null)
            {
                _messageModel.AddError("Token sem informações de usuário.");
                return null;
            }

            return codigoUsuario;
        }

        private ClaimsPrincipal ParseToken(string token)
        {
            var tokenValidationParameters = CreateTokenValidationParameters();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                var jwtToken = (JwtSecurityToken)securityToken;

                if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    _messageModel.AddError("Token inválido.");
                    return null;
                }

                return principal;
            }
            catch (Exception ex)
            {
                _messageModel.AddWarning(ex.Message);
                return null;
            }
        }

        private TokenValidationParameters CreateTokenValidationParameters()
        {
            var chaveSecreta = _configuration.GetSecretKey();

            var secretKey = Encoding.UTF8.GetBytes(chaveSecreta);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateLifetime = false
            };

            return tokenValidationParameters;
        }

        public ClaimsPrincipal ObterClaimPrincipal(string token)
        {
            var claimsPrincipal = ParseToken(token);
            return claimsPrincipal;
        }

        public string CriarJwtToken(string secretKey, string audience, string issuer, IEnumerable<Claim> claims, DateTime expiration)
        {
            // Secret Key como array de bytes
            // Gerar chave privada e assinar o token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            //   // Gerar a assinatura digital do token
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Gerar o token
            var tokenDescriptor = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<string> GerarRefreshTokenAsync()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var refreshToken = Convert.ToBase64String(randomNumber);

            // Opcional: Validar no banco se já existe um igual (chance quase zero, mas por segurança)
            var exists = await _loginRepository.ObterRefreshTokenDoUsuario(refreshToken);
            if (exists)
            {
                return await GerarRefreshTokenAsync(); // Recursividade controlada
            }

            return refreshToken;
        }
    }
}
