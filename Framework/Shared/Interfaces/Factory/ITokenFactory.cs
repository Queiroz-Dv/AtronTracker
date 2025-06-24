using Shared.DTO.API;

namespace Shared.Interfaces.Factory
{
    public interface ITokenFactory
    {
        Task<InfoToken> CriarTokenAsync(DadosDoUsuario usuario);
    }
}