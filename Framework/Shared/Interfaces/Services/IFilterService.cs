namespace Shared.Interfaces.Services
{
    public interface IFilterService<T>
    {
        List<T> ApplyFilter(List<T> items, string filter, string keyToSearch = null);
    }
}