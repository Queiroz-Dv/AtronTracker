namespace Shared.Application.Interfaces.Service;

public interface IRefreshTokenUnicidadeService
{
    Task<bool> ExisteAsync(string refreshToken);
}
