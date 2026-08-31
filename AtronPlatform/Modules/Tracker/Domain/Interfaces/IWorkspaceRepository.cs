#nullable enable

using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IWorkspaceRepository
{
    Task<bool> UsuarioPossuiWorkspaceAsync(string usuarioCodigo);
    Task<bool> EmpresaPossuiWorkspaceAsync(string empresaCodigo);
    Task<bool> CriarInicialAsync(Workspace workspace);
}
