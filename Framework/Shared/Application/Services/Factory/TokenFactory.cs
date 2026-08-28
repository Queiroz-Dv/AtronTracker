using Microsoft.Extensions.Configuration;
using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;
using Shared.Application.Interfaces.Service;
using Shared.Extensions;
using System.Security.Claims;

namespace Shared.Application.Services.Factory
{
    public class TokenFactory(IConfiguration configuration,
        IRefreshTokenUnicidadeService refreshTokenUnicidadeService) :
        TokenBuilder(configuration, refreshTokenUnicidadeService), ITokenFactoryService
    {
        public async Task<DadosDeTokenComRefreshToken> ObterDadosDoTokenAsync(DadosComplementaresDoUsuarioDTO dadosComplementaresDoUsuarioDTO)
        {
            var tempoDoToken = dadosComplementaresDoUsuarioDTO.DadosDoToken.ExpiracaoDoToken;
            var tempoDoRefreshToken = dadosComplementaresDoUsuarioDTO.DadosDoToken.ExpiracaoDoRefreshToken;

            var dadosDoUsuario = dadosComplementaresDoUsuarioDTO.DadosDoUsuario;

            var claims = JwtConfiguration.GetClaims(dadosComplementaresDoUsuarioDTO);
            var token = CriarJwtToken(claims, tempoDoToken);
            var refreshToken = await CriarRefreshToken();

            return new DadosDeTokenComRefreshToken
            {
                TokenDTO = new DadosDoTokenDTO(token, tempoDoToken, dadosDoUsuario.CodigoDoUsuario),
                RefrehTokenDTO = new DadosDoRefrehTokenDTO(refreshToken, tempoDoRefreshToken)
            };
        }

        public ClaimsPrincipal ObterClaimPrincipal(string token)
        {
            return CriarClaimPrincipal(token);
        }
    }
}
