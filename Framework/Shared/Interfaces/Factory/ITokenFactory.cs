using Shared.DTO.API;
using System.Security.Claims;

namespace Shared.Interfaces.Factory
{
    public interface ITokenFactory
    {
        Task<DadosDeTokenComRefreshToken> ObterDadosDoTokenAsync(DadosComplementaresDoUsuarioDTO usuario);

        ClaimsPrincipal ObterClaimPrincipal(string token);
    }
}