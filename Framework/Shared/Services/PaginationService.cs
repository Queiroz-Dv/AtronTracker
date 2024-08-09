using Shared.DTO;

namespace Shared.Services
{
    public class PaginationService
    {
        private int CurrentPage { get; set; }
        private int ItemsPerPage { get; set; }
        public bool ForceFilter { get; set; }
        public string FilterBy { get; set; }

        public PageInfoDTO Paginate<T>(IEnumerable<T> allItens,
            int currentPage,
            string controllerRoute,
            string filter = "",
            string action = nameof(Index),
            int itemsPerPage = 5)
        {
            var filteredItens = ApplyFilter(allItens, filter);
            var totalItems = filteredItens.Count();
            var totalPages = (int)Math.Ceiling((decimal)totalItems / itemsPerPage);
            var startPage = currentPage - 2;
            var endPage = currentPage + 2;

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

            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;

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

            return pageInfo;
        }

        public List<T> GetEntityPaginated<T>(List<T> values, string filter = "")
        {
            var filteredItems = ForceFilter ? ApplyFilter(values, FilterBy) : ApplyFilter(values, filter);

            return PaginateEntities(filteredItems);
        }

        private List<T> PaginateEntities<T>(IEnumerable<T> items)
        {
            var entities = items.Skip((CurrentPage - 1) * ItemsPerPage)
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

            var entities = items.Where(item => item?.GetType().GetProperty("Codigo")?.GetValue(item)?.ToString()?.Contains(filter) ?? false);
            return entities;
        }
    }
}