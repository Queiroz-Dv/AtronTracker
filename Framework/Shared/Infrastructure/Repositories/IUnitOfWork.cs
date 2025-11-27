using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Repositories
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        /// <summary>
        /// Inicia uma transação de banco de dados.
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Confirma todas as alterações feitas no escopo da transação.
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// Desfaz todas as alterações em caso de erro.
        /// </summary>
        Task RollbackAsync();
    }
}
