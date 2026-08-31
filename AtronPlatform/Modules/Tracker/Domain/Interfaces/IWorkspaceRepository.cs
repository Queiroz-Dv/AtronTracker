#nullable enable

using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IWorkspaceRepository
{
    Task<Workspace?> ObterPorIdAsync(int workspaceId);
    Task<Workspace?> ObterPorIdDoUsuarioAsync(int workspaceId, string usuarioCodigo);
    Task<IReadOnlyCollection<Workspace>> ObterPorUsuarioAsync(string usuarioCodigo);
    Task<bool> UsuarioPossuiWorkspaceAsync(string usuarioCodigo);
    Task<bool> UsuarioPertenceAoWorkspaceAsync(int workspaceId, string usuarioCodigo);
    Task<bool> EmpresaPossuiWorkspaceAsync(string empresaCodigo);
    Task<bool> CriarInicialAsync(Workspace workspace);
}
