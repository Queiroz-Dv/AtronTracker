using Microsoft.Extensions.Configuration;
using Shared.Application.Interfaces.Service;
using Shared.DTO.API;
using Shared.Extensions;
using System.Security.Claims;

namespace Shared.Services.Factory
{
    public class TokenFactory : TokenBuilder, ITokenFactoryService
    {
        public TokenFactory(IConfiguration configuration,
            IAccessorService serviceAccessor) : base(configuration, serviceAccessor) { }

        public async Task<DadosDeTokenComRefreshToken> ObterDadosDoTokenAsync(DadosComplementaresDoUsuarioDTO dadosComplementaresDoUsuarioDTO)
        {
            var tempoDoToken = dadosComplementaresDoUsuarioDTO.DadosDoToken.ExpiracaoDoToken;
            var tempoDoRefreshToken = dadosComplementaresDoUsuarioDTO.DadosDoToken.ExpiracaoDoRefreshToken;

            var dadosDoUsuario = dadosComplementaresDoUsuarioDTO.DadosDoUsuario;

            var claims = JwtConfiguration.GetClaims(dadosDoUsuario);
            var token = CriarJwtToken(claims, tempoDoToken);
            var refreshToken = await CriarRefreshToken();

            return new DadosDeTokenComRefreshToken
            {
                TokenDTO = new DadosDoTokenDTO(token, tempoDoToken),
                RefrehTokenDTO = new DadosDoRefrehTokenDTO(refreshToken, tempoDoRefreshToken)
            };
        }

        public ClaimsPrincipal ObterClaimPrincipal(string token)
        {
            return CriarClaimPrincipal(token);
        }
    }
}