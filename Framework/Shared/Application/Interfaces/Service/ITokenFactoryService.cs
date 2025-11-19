using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;
using System.Security.Claims;

namespace Shared.Application.Interfaces.Service
{
    public interface ITokenFactoryService
    {
        Task<DadosDeTokenComRefreshToken> ObterDadosDoTokenAsync(DadosComplementaresDoUsuarioDTO usuario);

        ClaimsPrincipal ObterClaimPrincipal(string token);
    }
}