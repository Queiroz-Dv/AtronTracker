using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using Shared.Models;

namespace Atron.Infrastructure.Repositories
{
    public abstract class RepositoryBase
    {
        protected ILiteFacade _facade;
        protected IServiceAccessor serviceAccessor;
        protected MessageModel MessageModel;

        public RepositoryBase(ILiteFacade facade, IServiceAccessor serviceAccessor = null)
        {
            _facade = facade;
            if (serviceAccessor != null)
            {
                this.serviceAccessor = serviceAccessor;
                MessageModel = serviceAccessor.ObterService<MessageModel>();
            }
        }

        internal ILiteDbContext GetLiteDbContext()
        {
            return _facade.LiteDbContext;
        }

        internal ILiteUnitOfWork GetLiteUnitOfWork()
        {
            return _facade.LiteUnitOfWork;
        }

        protected void BeginTransaction()
        {
            var uow = GetLiteUnitOfWork();
            if (uow != null)
            {
                uow.BeginTransaction();
            }
        }

        private void Commit()
        {
            var uow = GetLiteUnitOfWork();
            if (uow != null)
            {
                uow.Commit();
            }
        }

        private void Rollback()
        {
            var uow = GetLiteUnitOfWork();
            if (uow != null)
            {
                uow.Rollback();
            }
        }

        public void CommitOrRollback(bool commit)
        {
            if (commit)
            {
                Commit();
            }
            else
            {
                Rollback();
            }
        }
    }
}