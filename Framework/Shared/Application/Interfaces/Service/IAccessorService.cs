namespace Shared.Application.Interfaces.Service
{
    public interface IAccessorService
    {
        T ObterService<T>() where T : class;
    }
}