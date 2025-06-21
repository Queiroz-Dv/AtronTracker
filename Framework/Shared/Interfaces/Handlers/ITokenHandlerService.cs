using System.Security.Claims;

namespace Shared.Interfaces.Handlers
{
    public interface ITokenHandlerService
    {
        ClaimsPrincipal ObterClaimPrincipal(string token);

        string ObterCodigoUsuarioPorClaim(string token);
    }
}