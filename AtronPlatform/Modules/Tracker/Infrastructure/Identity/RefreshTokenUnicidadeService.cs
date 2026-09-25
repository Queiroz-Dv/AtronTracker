using Domain.Interfaces.Identity;
using Shared.Application.Security;
using Shared.Application.Interfaces.Service;

namespace Infrastructure.Identity;

internal sealed class RefreshTokenUnicidadeService(
    IUsuarioIdentityRepository usuarioIdentityRepository)
    : IRefreshTokenUnicidadeService
{
    public Task<bool> ExisteAsync(string refreshToken)
    {
        return usuarioIdentityRepository.RefreshTokenExisteRepositoryAsync(RefreshTokenHash.Obter(refreshToken));
    }
}
