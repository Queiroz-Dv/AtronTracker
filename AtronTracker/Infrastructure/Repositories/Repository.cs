using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
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

        public async Task<bool> AtualizarRepositoryAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            var atualizado = await _context.SaveChangesAsync();
            return atualizado > 0;
        }

        public async Task<bool> AtualizarRepositoryAsync(int id, TEntity entity)
        {
            TEntity entityBd = await _dbSet.FindAsync(id);
            entityBd = entity;
            _dbSet.Update(entityBd);
            var atualizado = await _context.SaveChangesAsync();
            return atualizado > 0;
        }

        public async Task<bool> CriarRepositoryAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            var gravado = await _context.SaveChangesAsync();
            return gravado > 0;
        }

        public async Task<TEntity> ObterPorCodigoRepositoryAsync(string codigo)
        {
            var entidade = await _dbSet.SingleOrDefaultAsync(e => EF.Property<string>(e, "Codigo") == codigo);
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

        public async Task<bool> RemoverRepositoryAsync(TEntity entity)
        {
            _context.Remove(entity);
            var removido = await _context.SaveChangesAsync();
            return removido > 0;
        }

        public async Task<bool> RemoverRepositoryAsync(int id)
        {
            TEntity entityBd = await _dbSet.FindAsync(id);
            _context.Remove(entityBd);

            var removido = await _context.SaveChangesAsync();
            return removido > 0;
        }
    }
}