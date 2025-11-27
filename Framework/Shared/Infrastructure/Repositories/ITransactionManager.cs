namespace Shared.Infrastructure.Repositories
{
    public interface ITransactionScope : IDisposable
    {
        void Complete(); // Commit
    }

    // O gerenciador que cria os escopos
    public interface ITransactionManager
    {
        ITransactionScope CreateScope();
    }
}