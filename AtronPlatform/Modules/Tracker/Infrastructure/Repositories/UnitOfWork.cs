using Infrastructure.Context;
using Shared.Application.Interfaces;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AtronDbContext _context;

        public UnitOfWork(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CommitAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
