using Shared.DTO.API;

namespace Shared.Interfaces
{
    public interface IApplicationTokenService
    {
        UserToken GenerateToken(DadosDoUsuario dadosDoUsuario);
    }
}