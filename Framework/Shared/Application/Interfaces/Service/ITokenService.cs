using Shared.DTO.API;
using System.Security.Claims;

namespace Shared.Application.Interfaces.Service
{
    public interface ITokenService
    {
        Task<DadosDeTokenComRefreshToken> ObterTokenComRefreshToken(DadosComplementaresDoUsuarioDTO dadosDoUsuario);

        Task<string> ObterCodigoUsuarioPorClaim(string token);

        ClaimsPrincipal ObterClaimPrincipal(string token);
    }
}