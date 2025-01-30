using Shared.Interfaces;

namespace Shared.Services
{
    public class FilterService<T> : IFilterService<T>
    {      
        public List<T> ApplyFilter(List<T> items, string filter, string? keyToSearch = null)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return items;
            }

            return keyToSearch != null
                            ? items.Where(item => item?.GetType()
                                                       .GetProperty(keyToSearch)?
                                                       .GetValue(item)?
                                                       .ToString()?
                                                       .Contains(filter) ?? false)
                                                       .ToList()
                : items.ToList();
        }
    }
}