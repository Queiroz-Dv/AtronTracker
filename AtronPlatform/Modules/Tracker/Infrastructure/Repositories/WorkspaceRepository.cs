#nullable enable

using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class WorkspaceRepository(AtronDbContext context) : IWorkspaceRepository
{
    public Task<Workspace?> ObterPorIdAsync(int workspaceId)
        => context.Workspaces
            .AsNoTracking()
            .Include(workspace => workspace.Empresa)
            .SingleOrDefaultAsync(workspace => workspace.Id == workspaceId);

    public async Task<IReadOnlyCollection<Workspace>> ObterPorUsuarioAsync(
        string usuarioCodigo)
        => await context.Workspaces
            .AsNoTracking()
            .Include(workspace => workspace.Empresa)
            .Where(workspace => workspace.Membros.Any(membro =>
                membro.UsuarioCodigo == usuarioCodigo))
            .OrderBy(workspace => workspace.Id)
            .ToListAsync();

    public Task<bool> UsuarioPossuiWorkspaceAsync(string usuarioCodigo)
        => context.MembrosWorkspace.AnyAsync(membro => membro.UsuarioCodigo == usuarioCodigo);

    public Task<bool> UsuarioPertenceAoWorkspaceAsync(
        int workspaceId,
        string usuarioCodigo)
        => context.MembrosWorkspace.AnyAsync(membro =>
            membro.WorkspaceId == workspaceId
            && membro.UsuarioCodigo == usuarioCodigo);

    public Task<bool> EmpresaPossuiWorkspaceAsync(string empresaCodigo)
        => context.Workspaces.AnyAsync(workspace => workspace.EmpresaCodigo == empresaCodigo);

    public async Task<bool> CriarInicialAsync(Workspace workspace)
    {
        context.Workspaces.Add(workspace);
        return await context.SaveChangesAsync() >= 2;
    }
}
