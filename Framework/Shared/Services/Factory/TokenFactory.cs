using Microsoft.Extensions.Configuration;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces.Factory;
using Shared.Interfaces.Handlers;

namespace Shared.Services.Factory
{
    public class TokenFactory : ITokenFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenBuilderService _tokenHandlerService;

        public TokenFactory(IConfiguration configuration, ITokenBuilderService tokenHandlerService)
        {
            _configuration = configuration;
            _tokenHandlerService = tokenHandlerService;
        }

        public async Task<InfoToken> CriarTokenAsync(DadosDoUsuario usuario)
        {
            var secretKey = _configuration.GetSecretKey();
            var audience = _configuration.GetAudience();
            var issuer = _configuration.GetIssuer();

            var claims = JwtConfiguration.GetClaims(usuario);

            var jwtToken = _tokenHandlerService.CriarJwtToken(secretKey, audience, issuer, claims, usuario.DadosDoToken.ExpiracaoDoToken);

            var refreshToken = await _tokenHandlerService.GerarRefreshTokenAsync();

            return new InfoToken
            {
                Token = jwtToken,
                Expires = usuario.DadosDoToken.ExpiracaoDoToken,
                RefreshToken = refreshToken,
                RefreshTokenExpireTime = usuario.DadosDoToken.ExpiracaoDoRefreshToken
            };
        }
    }

}
