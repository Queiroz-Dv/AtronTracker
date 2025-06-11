using Atron.Domain.Interfaces.ApplicationInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Services
{
    public class ApplicationTokenService : IApplicationTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILoginApplicationRepository _loginRepository;

        public ApplicationTokenService(IConfiguration configuration, ILoginApplicationRepository loginRepository)
        {
            _configuration = configuration;
            _loginRepository = loginRepository;
        }

        public async Task<InfoToken> GerarToken(DadosDoUsuario dadosDoUsuario)
        {
            // Objeto de configuração do token
            var appSecurityToken = new ApplicationSecurityToken()
            {
                SecretKey = _configuration.GetSecretKey(), // Chave secreta
                Issuer = _configuration.GetIssuer(),      // Emissor
                Audience = _configuration.GetAudience(),  // Audiência
                Claims = JwtConfiguration.GetClaims(dadosDoUsuario) // Claims
            };

            // Secret Key como array de bytes
            var secretKeyEncoding = Encoding.UTF8.GetBytes(appSecurityToken.SecretKey);

            // Gerar chave privada e assinar o token
            var privateKey = new SymmetricSecurityKey(secretKeyEncoding);

            // Gerar a assinatura digital do token
            var userSigningCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            // Gerar o token
            var token = new JwtSecurityToken(
                issuer: appSecurityToken.Issuer,
                audience: appSecurityToken.Audience,
                claims: appSecurityToken.Claims,
                expires: dadosDoUsuario.DadosDoToken.ExpiracaoDoToken,
                signingCredentials: userSigningCredentials
            );

            try
            {
                var tokenCreated = new JwtSecurityTokenHandler().WriteToken(token);

                // Retornar o token e o refresh token
                var userToken = new InfoToken()
                {
                    Token = tokenCreated,
                    Expires = dadosDoUsuario.DadosDoToken.ExpiracaoDoToken,
                    RefreshToken = await CriarRefreshToken(),
                    RefreshTokenExpireTime = dadosDoUsuario.DadosDoToken.ExpiracaoDoRefreshToken
                };

                return userToken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<string> ObterChaveSecreta()
        {
            return Task.FromResult(_configuration.GetSecretKey());
        }

        private async Task<string> CriarRefreshToken()
        {
            string token;
            do
            {
                var bytes = RandomNumberGenerator.GetBytes(64);
                token = Convert.ToBase64String(bytes);
            }
            while (await _loginRepository.ObterRefreshTokenDoUsuario(token));

            return token;
        }
    }
}