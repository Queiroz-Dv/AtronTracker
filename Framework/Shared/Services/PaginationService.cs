using Shared.DTO;

namespace Shared.Services
{
    public class PaginationService
    {
        private int CurrentPage { get; set; }
        private int ItemsPerPage { get; set; }

        public PageInfoDTO Paginate<T>(IEnumerable<T> items,
            int currentPage,
            string controllerRoute,
            string filter = "",
            string action = nameof(Index),
            int itemsPerPage = 5)
        {
            //var filteredItems = ApplyFilter(items, filter);
            var totalItems = items.Count();
            var totalPages = (int)Math.Ceiling((decimal)totalItems / itemsPerPage);
            var startPage = currentPage - 2;
            var endPage = currentPage + 1;

            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }

            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 5)
                {
                    startPage = endPage - 4;
                }
            }

            var pageInfo = new PageInfoDTO
            {
                TotalItems = totalItems,
                ItemsPerPage = itemsPerPage,
                CurrentPage = currentPage,
                CurrentController = controllerRoute,
                Action = action,
                Filter = filter,
                StartPage = startPage,
                EndPage = endPage
            };

            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            return pageInfo;
        }

        public List<T> GetEntityPaginated<T>(List<T> values, string filter = "")
        {
            var filteredItems = ApplyFilter(values, filter);
            var entities = filteredItems.Skip((CurrentPage - 1) * ItemsPerPage)
                            .Take(ItemsPerPage)
                            .ToList();
            return entities;
        }

        private IEnumerable<T> ApplyFilter<T>(IEnumerable<T> items, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return items;
            }

            // Assuming T has a property called Codigo
            return items.Where(item => item.GetType().GetProperty("Codigo")?.GetValue(item)?.ToString().Contains(filter.ToUpper()) ?? false);
        }
    }
}