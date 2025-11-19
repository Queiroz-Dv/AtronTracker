using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;

namespace Application.Interfaces.Services
{
    public interface ICacheUsuarioService
    {
        void GravarCacheDeAcessoTokenInfo(DadosComplementaresDoUsuarioDTO dadosDoUsuario, DadosDeTokenComRefreshToken tokenComRefreshToken);

        DadosDeTokenComRefreshToken ObterDadosDoTokenPorCodigoUsuario(string codigoUsuario);
    }
}