#nullable enable

using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace Infrastructure.Repositories;

public sealed class ConviteWorkspaceRepository(AtronDbContext context)
    : IConviteWorkspaceRepository
{
    public async Task<bool> CriarAsync(ConviteWorkspace convite)
    {
        context.ConvitesWorkspace.Add(convite);
        return await context.SaveChangesAsync() > 0;
    }

    public Task<ConviteWorkspace?> ObterAtivoPorHashAsync(string identificadorHash)
    {
        var agora = DateTime.UtcNow.SemTimezone();

        return context.ConvitesWorkspace
            .AsNoTracking()
            .Include(convite => convite.Workspace)
            .ThenInclude(workspace => workspace.Empresa)
            .Where(convite =>
                convite.IdentificadorHash == identificadorHash
                && convite.UtilizadoEm == null
                && convite.ExpiraEm >= agora)
            .SingleOrDefaultAsync();
    }

    public async Task<bool> ConsumirAsync(
        ConviteWorkspace convite,
        string usuarioCodigo)
    {
        var agora = DateTime.UtcNow.SemTimezone();
        var atualizados = await context.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE ""ConvitesWorkspace""
               SET ""UtilizadoEm"" = {agora},
                   ""UtilizadoPorUsuarioCodigo"" = {usuarioCodigo}
             WHERE ""Id"" = {convite.Id}
               AND ""UtilizadoEm"" IS NULL
               AND ""ExpiraEm"" >= {agora}");

        if (atualizados != 1)
            return false;

        context.MembrosWorkspace.Add(new MembroWorkspace
        {
            WorkspaceId = convite.WorkspaceId,
            UsuarioCodigo = usuarioCodigo
        });

        return await context.SaveChangesAsync() > 0;
    }
}
