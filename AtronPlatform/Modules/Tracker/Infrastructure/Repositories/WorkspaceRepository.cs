using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WorkspaceRepository(AtronDbContext context) : IWorkspaceRepository
    {
        private readonly AtronDbContext _context = context;

        public async Task<bool> CriarWorkspace(Workspace workspace)
        {
            await _context.Workspaces.AddAsync(workspace);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Workspace> ObterWorkspacePorCodigo(string codigo)
        {
            return await _context.Workspaces.Where(work => work.Codigo == codigo).FirstOrDefaultAsync();
        }

        public async Task<Workspace> ObterWorkspacePorResponsavelEmailAsync(string codigoResponsavel, string email)
        {
            return await _context.Workspaces.Where(
                work => work.ResponsavelCodigo == codigoResponsavel &&
                work.ResponsavelEmail == email).FirstOrDefaultAsync();
        }
    }
}