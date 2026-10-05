using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces.UsuarioInterfaces
{
    public interface IWorkspaceRepository
    {
        Task<bool> CriarWorkspace(Workspace workspace);
        Task<bool> AtualizarResponsavelAsync(string workspaceCodigo, int responsavelId);
        Task<Workspace> ObterWorkspacePorResponsavelEmailAsync(string codigoResponsavel, string email);
        Task<Workspace> ObterWorkspacePorMembroAsync(string usuarioCodigo);
        Task<Workspace> ObterWorkspacePorCodigo(string codigo);
    }
}
