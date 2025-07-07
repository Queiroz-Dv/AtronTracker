namespace Shared.Interfaces.Accessor
{
    public interface IServiceAccessor
    {
        T ObterService<T>() where T : class;
    }
}