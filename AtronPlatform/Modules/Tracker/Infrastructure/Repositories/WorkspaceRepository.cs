#nullable enable

using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class WorkspaceRepository(AtronDbContext context) : IWorkspaceRepository
{
    public Task<bool> UsuarioPossuiWorkspaceAsync(string usuarioCodigo)
        => context.MembrosWorkspace.AnyAsync(membro => membro.UsuarioCodigo == usuarioCodigo);

    public Task<bool> EmpresaPossuiWorkspaceAsync(string empresaCodigo)
        => context.Workspaces.AnyAsync(workspace => workspace.EmpresaCodigo == empresaCodigo);

    public async Task<bool> CriarInicialAsync(Workspace workspace)
    {
        context.Workspaces.Add(workspace);
        return await context.SaveChangesAsync() >= 2;
    }
}
