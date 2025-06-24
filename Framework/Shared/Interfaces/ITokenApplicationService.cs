using Shared.DTO.API;

namespace Shared.Interfaces
{
    public interface ITokenApplicationService
    {
        Task<InfoToken> CriarTokenParaUsuario(DadosDoUsuario dadosDoUsuario);        
    }
}