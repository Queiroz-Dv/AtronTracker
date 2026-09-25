using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces.UsuarioInterfaces
{
    public interface IWorkspaceRepository
    {
        Task<bool> CriarWorkspace(Workspace workspace);
        Task<Workspace> ObterWorkspacePorResponsavelEmailAsync(string codigoResponsavel, string email);
        Task<Workspace> ObterWorkspacePorCodigo(string codigo);
    }
}