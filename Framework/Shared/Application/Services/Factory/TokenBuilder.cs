using Domain.Interfaces.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Application.Interfaces.Service;
using Shared.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Application.Services.Factory
{
    public class TokenBuilder
    {
        private readonly IConfiguration _configuration;
        private readonly IAccessorService _serviceAccessor;

        public TokenBuilder(IConfiguration configuration,
                            IAccessorService serviceAccessor)
        {
            _configuration = configuration;
            _serviceAccessor = serviceAccessor;
        }

        protected string CriarJwtToken(IEnumerable<Claim> claims, DateTime expiration)
        {
            var secretKey = _configuration.GetSecretKey();
            var audience = _configuration.GetAudience();
            var issuer = _configuration.GetIssuer();

            // Gerar chave privada e assinar o token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Gerar a assinatura digital do token
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

        protected async Task<string> CriarRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var refreshToken = Convert.ToBase64String(randomNumber);

            // Opcional: Validar no banco se já existe um igual (chance quase zero, mas por segurança)
            var userIdentityRepo = _serviceAccessor.ObterService<IUsuarioIdentityRepository>();
            var refreshTokenExiste = await userIdentityRepo.RefreshTokenExisteRepositoryAsync(refreshToken);

            if (refreshTokenExiste)
            {
                return await CriarRefreshToken(); // Recursividade controlada
            }

            return refreshToken;
        }

        protected ClaimsPrincipal CriarClaimPrincipal(string token)
        {
            var tokenValidationParameters = CreateTokenValidationParameters();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                var jwtToken = (JwtSecurityToken)securityToken;

                if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch (Exception)
            {
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
    }
}