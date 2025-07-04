using System.Security.Claims;

namespace Shared.Interfaces.Handlers
{
    public interface ITokenBuilderService
    {
        ClaimsPrincipal ObterClaimPrincipal(string token);

        string ObterCodigoUsuarioPorClaim(string token);

        string CriarJwtToken(string secretKey, string audience, string issuer, IEnumerable<Claim> claims, DateTime expiration);

        Task<string> GerarRefreshTokenAsync();
    }
}