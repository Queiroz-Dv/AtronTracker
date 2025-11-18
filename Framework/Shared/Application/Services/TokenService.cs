using Shared.Application.Interfaces.Service;
using Shared.DTO.API;
using System.Security.Claims;

namespace Shared.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenFactoryService _tokenFactory;

        public TokenService(ITokenFactoryService tokenFactory)
        {
            _tokenFactory = tokenFactory;
        }

        public ClaimsPrincipal ObterClaimPrincipal(string token)
        {
            return _tokenFactory.ObterClaimPrincipal(token);
        }

        public Task<string> ObterCodigoUsuarioPorClaim(string token)
        {
            var claimsPrincipal = _tokenFactory.ObterClaimPrincipal(token);

            if (claimsPrincipal == null) return null;

            return Task.FromResult(claimsPrincipal.Claims.FirstOrDefault(cls => cls.Type == ClaimCode.CODIGO_USUARIO).Value);
        }

        public async Task<DadosDeTokenComRefreshToken> ObterTokenComRefreshToken(DadosComplementaresDoUsuarioDTO dadosDoUsuario)
        {
            return await _tokenFactory.ObterDadosDoTokenAsync(dadosDoUsuario);
        }
    }
}