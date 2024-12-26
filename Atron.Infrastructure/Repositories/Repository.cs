using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly AtronDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(AtronDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task AtualizarRepositoryAsync(TEntity entity)
        {            
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task CriarRepositoryAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity> ObterPorCodigoRepositoryAsync(string codigo)
        {
            var entidade = await _dbSet.FindAsync(codigo);
            return entidade;
        }

        public async Task<TEntity> ObterPorIdRepositoryAsync(int id)
        {
            var entidade = await _dbSet.FindAsync(id);
            return entidade;
        }

        public async Task<IEnumerable<TEntity>> ObterTodosRepositoryAsync()
        {
            var entidades = await _dbSet.AsNoTracking().ToListAsync();
            return entidades;
        }

        public async Task RemoverRepositoryAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}