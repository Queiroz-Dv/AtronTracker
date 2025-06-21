using Shared.DTO.API;

namespace Shared.Interfaces
{
    public interface IApplicationTokenService
    {
        Task<InfoToken> GerarToken(DadosDoUsuario dadosDoUsuario);

        Task<string> ObterChaveSecreta();
    }
}