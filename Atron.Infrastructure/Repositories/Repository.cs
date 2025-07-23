using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using LiteDB;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Accessor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class Repository<TEntity> : RepositoryBase, IRepository<TEntity> where TEntity : class
    {
        protected readonly IDataSet<TEntity> _dbSet;
        
        public string CollectionName { get; set; }

        public Repository(ILiteFacade liteFacade, IServiceAccessor serviceAccessor) : base(liteFacade, serviceAccessor)
        {
        }

        public async Task<bool> AtualizarRepositoryAsync(TEntity entity)
        {
            BeginTransaction();
            try
            {
                var atualizado = await _dbSet.UpdateAsync(entity);
                CommitOrRollback(atualizado);
                return atualizado;
            }
            catch (Exception ex)
            {
                CommitOrRollback(false);
                // Não para a execução, mas registra o erro
                MessageModel.AdicionarErro($"Erro ao atualizar entidade: {ex.Message}");
                return false;
            }

        }

        public async Task<bool> AtualizarRepositoryAsync(int id, TEntity entity)
        {
            TEntity entityBd = await _dbSet.FindByIdAsync(id);
            entityBd = entity;

            BeginTransaction();
            try
            {
                var atualizado = await _dbSet.UpdateAsync(entity);
                CommitOrRollback(atualizado);
                return atualizado;
            }
            catch (Exception ex)
            {
                CommitOrRollback(false);
                // Não para a execução, mas registra o erro
                MessageModel.AdicionarErro($"Erro ao atualizar entidade: {ex.Message}");
                return false;
            }

        }

        public async Task<bool> CriarRepositoryAsync(TEntity entity)
        {
            BeginTransaction();
            try
            {
                var criado = await _dbSet.InsertAsync(entity);
                CommitOrRollback(criado != null);
                return criado > 0;
            }
            catch (Exception ex)
            {
                CommitOrRollback(false);
                // Não para a execução, mas registra o erro
                MessageModel.AdicionarErro($"Erro ao criar entidade: {ex.Message}");
                return false;
                throw;
            }

        }

        public async Task<TEntity> ObterPorCodigoRepositoryAsync(string codigo)
        {
            var entidade = await _dbSet.FindOneAsync(e => EF.Property<string>(e, "Codigo") == codigo);
            return entidade;
        }

        public async Task<TEntity> ObterPorIdRepositoryAsync(int id)
        {
            var entidade = await _dbSet.FindByIdAsync(id);
            return entidade;
        }

        public async Task<IEnumerable<TEntity>> ObterTodosRepositoryAsync()
        {
            var entidades = await _dbSet.FindAllAsync();
            return entidades;
        }

        public async Task<bool> RemoverRepositoryAsync(int id)
        {
            BeginTransaction();
            try
            {
                var removido = await _dbSet.DeleteAsync(id);
                CommitOrRollback(removido);
                return removido;
            }
            catch (Exception ex)
            {
                CommitOrRollback(false);
                MessageModel.AdicionarErro($"Erro ao remover entidade: {ex.Message}");
                return false;
            }
        }

        public Task RemoverRepositoryAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}