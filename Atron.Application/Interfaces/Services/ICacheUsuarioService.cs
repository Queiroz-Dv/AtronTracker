using Shared.DTO.API;

namespace Atron.Application.Interfaces.Services
{
    public interface ICacheUsuarioService
    {
        void GravarCacheDeAcessoTokenInfo(DadosComplementaresDoUsuarioDTO dadosDoUsuario, DadosDeTokenComRefreshToken tokenComRefreshToken);

        DadosDeTokenComRefreshToken ObterDadosDoTokenPorCodigoUsuario(string codigoUsuario);
    }
}