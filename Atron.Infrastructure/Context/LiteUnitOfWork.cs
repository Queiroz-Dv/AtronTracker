using Atron.Infrastructure.Interfaces;
using LiteDB;

namespace Atron.Infrastructure.Context
{
    public class LiteUnitOfWork : ILiteUnitOfWork
    {
        private readonly LiteDatabase _database;

        public LiteUnitOfWork(LiteDatabase database)
        {
            _database = database;
        }

        public void BeginTransaction() => _database.BeginTrans();
        public void Commit() => _database.Commit();
        public void Rollback() => _database.Rollback();
        public void Dispose() { }
    }
}