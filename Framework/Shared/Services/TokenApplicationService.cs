using Shared.DTO.API;
using Shared.Interfaces;
using Shared.Interfaces.Factory;

namespace Shared.Services
{
    public class TokenApplicationService : ITokenApplicationService
    {
        private readonly ITokenFactory _tokenFactory;        

        public TokenApplicationService(ITokenFactory tokenFactory)
        {
            _tokenFactory = tokenFactory;
        }

        public async Task<InfoToken> CriarTokenParaUsuario(DadosDoUsuario dadosDoUsuario)
        {
            return await _tokenFactory.CriarTokenAsync(dadosDoUsuario);
        }
    }
}