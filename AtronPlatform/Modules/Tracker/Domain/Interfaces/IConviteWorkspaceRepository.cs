using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IConviteWorkspaceRepository
{
    Task<bool> CriarAsync(ConviteWorkspace convite);
    Task<ConviteWorkspace?> ObterAtivoPorHashAsync(string identificadorHash);
    Task<bool> ConsumirAsync(
        ConviteWorkspace convite,
        string usuarioCodigo);
}
