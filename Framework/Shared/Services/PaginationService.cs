using Shared.DTO;
using Shared.Interfaces;

namespace Shared.Services
{
    public class PaginationService<T> : IPaginationService<T>
    {
        public PaginationService()
        {
            PageInfo = new PageInfoDTO();
            Entities = new List<T>();
        }

        private List<T> Entities { get; set; }
        private int CurrentPage { get; set; }
        private int ItemsPerPage { get; set; }
        public bool ForceFilter { get; set; }
        public string FilterBy { get; set; }
        public PageInfoDTO PageInfo { get; set; }

        private void Paginate<T>(IEnumerable<T> allItens,
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

            PageInfo = pageInfo;
        }

        public void ConfigureEntityPaginated(List<T> values, string filter = "")
        {
            var filteredItems = ForceFilter ? ApplyFilter(values, FilterBy) : ApplyFilter(values, filter);

            Entities = PaginateEntities(filteredItems);            
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

        private List<T> PaginateEntities<T>(IEnumerable<T> items)
        {
            var entities = items.Skip((CurrentPage - 1) * ItemsPerPage)
                                .Take(ItemsPerPage)
                                .ToList();
            return entities;
        }

        public List<T> GetEntitiesFilled()
        {
            return Entities;
        }

        public void Paginate(List<T> items, int itemPage, string controllerName, string filter)
        {
            Paginate<T>(items, itemPage, controllerName, filter);
        }
    }
}