using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;

namespace Atron.Infrastructure.Repositories
{
    public class PermissaoRepository : Repository<Permissao>, IPermissaoRepository
    {
        private readonly AtronDbContext _context;

        public PermissaoRepository(AtronDbContext context) : base(context) { }
    }
}