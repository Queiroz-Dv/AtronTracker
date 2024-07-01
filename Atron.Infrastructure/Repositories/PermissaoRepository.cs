using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class PermissaoRepository : IPermissaoRepository
    {
        private readonly AtronDbContext _context;

        public PermissaoRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<Permissao> CriarPermissaoRepositoryAsync(Permissao permissao)
        {
            await _context.AddAsync(permissao);
            await _context.SaveChangesAsync();
            return permissao;
        }

        public async Task<IEnumerable<Permissao>> ObterPermissoesRepositoryAsync()
        {
            var permissoes = await _context.Permissoes.AsNoTracking().ToListAsync();
            return permissoes;
        }
    }
}