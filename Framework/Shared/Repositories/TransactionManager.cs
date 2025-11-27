using Shared.Infrastructure.Repositories;
using System.Transactions;

namespace Shared.Repositories
{
    public class DistributedTransactionScope : ITransactionScope
    {
        private readonly TransactionScope _scope;

        public DistributedTransactionScope()
        {
            var options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };
            
            _scope = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled);
        }

        public void Complete()
        {
            _scope.Complete();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }

    public class TransactionManager : ITransactionManager
    {
        public static TimeSpan MaximumTimeout { get; set; } = TimeSpan.FromMinutes(3);

        public ITransactionScope CreateScope()
        {
            return new DistributedTransactionScope();
        }
    }
}
