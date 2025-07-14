using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Context
{
    public interface IDataSet<T> where T : class
    {
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<T> FindOneAsync(Expression<Func<T, bool>> predicate);
        Task<T> FindByIdAsync(BsonValue id);
        Task<IEnumerable<T>> FindAllAsync();
        Task<BsonValue> InsertAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(BsonValue id);
    }


    public class LiteDbSet<T> : IDataSet<T> where T : class
    {
        private readonly ILiteCollection<T> _collection;

        public LiteDbSet(ILiteDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public Task<T> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            var result = _collection.Find(predicate).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            var exists = _collection.Exists(predicate);
            return Task.FromResult(exists);
        }

        public Task<T> FindByIdAsync(BsonValue id) => Task.FromResult(_collection.FindById(id));
        public Task<IEnumerable<T>> FindAllAsync() => Task.FromResult(_collection.FindAll().AsEnumerable());
        public Task<BsonValue> InsertAsync(T entity) => Task.FromResult(_collection.Insert(entity));
        public Task<bool> UpdateAsync(T entity) => Task.FromResult(_collection.Update(entity));
        public Task<bool> DeleteAsync(BsonValue id) => Task.FromResult(_collection.Delete(id));
    }
}