using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public readonly AtronDbContext _context;
        public readonly ILiteDbContext _liteContext;

        private readonly DbSet<TEntity> _dbSet;

        public Repository(AtronDbContext context, ILiteDbContext liteContext)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _liteContext = liteContext;
        }

        public async Task AtualizarRepositoryAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarRepositoryAsync(int id, TEntity entity)
        {
            TEntity entityBd = await _dbSet.FindAsync(id);
            entityBd = entity;
            _dbSet.Update(entityBd);
            await _context.SaveChangesAsync();
        }

        public async Task CriarRepositoryAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
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

        public async Task RemoverRepositoryAsync(TEntity entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        protected void RunInTransaction(Action operation)
        {
            _liteContext.BeginTransaction();

            try
            {
                operation();
                _liteContext.Commit();
            }
            catch 
            {
                _liteContext.Rollback();
                throw;
            }
        }
    }
}