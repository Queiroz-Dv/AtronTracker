namespace Atron.Infrastructure.Interfaces
{
    public interface ILiteTransactions
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}