using Shared.DTO.API;

namespace Shared.Interfaces
{
    public interface IApplicationTokenService
    {
        UserToken GenerateToken( string usuarioNome, string codigoDeUsuario, string email);                
    }
}