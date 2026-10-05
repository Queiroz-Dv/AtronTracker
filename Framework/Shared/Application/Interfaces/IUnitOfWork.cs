namespace Shared.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync();
    }
}
