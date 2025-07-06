using Shared.DTO.API;
using Shared.Interfaces.Factory;
using Shared.Interfaces.Services;
using System.Security.Claims;

namespace Shared.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenFactory _tokenFactory;

        public TokenService(ITokenFactory tokenFactory)
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