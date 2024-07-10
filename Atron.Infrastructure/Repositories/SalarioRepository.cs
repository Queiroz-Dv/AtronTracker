using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;

namespace Atron.Infrastructure.Repositories
{
    public class SalarioRepository : Repository<Salario>, ISalarioRepository
    {
        private AtronDbContext _context;

        public SalarioRepository(AtronDbContext context) : base(context) { }
    }
}