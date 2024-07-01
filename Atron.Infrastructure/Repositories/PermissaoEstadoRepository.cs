using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class PermissaoEstadoRepository : IPermissaoEstadoRepository
    {
        private readonly AtronDbContext _context;

        public PermissaoEstadoRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<List<PermissaoEstado>> ObterTodosPermissaoEstadoRepositoryAsync()
        {
            var estados = await _context.PermissoesEstados.AsNoTracking().ToListAsync();
            return estados;
        }
    }
}