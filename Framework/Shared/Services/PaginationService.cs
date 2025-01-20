using Shared.DTO;
using Shared.Interfaces;

namespace Shared.Services
{
    /// <summary>
    /// Classe de implementação dos serviços de paginação
    /// </summary>
    /// <typeparam name="T">Entidade que será utilizada no processo</typeparam>
    public class PaginationService<T> : IPaginationService<T>
    {
        private IPageRequestService PageRequestService;
        private readonly IFilterService<T> _filterService;

        private PageInfoDTO<T> Page { get; set; }
        
        public PaginationService(IFilterService<T> filterService, IPageRequestService pageRequestService)
        {
            _filterService = filterService;
            PageRequestService = pageRequestService;
            Page = new PageInfoDTO<T>();
        }

        public void ConfigurePagination(PageInfoDTO<T> pageInfo)
        {
            var itensFiltrados = _filterService.ApplyFilter(pageInfo.Entities, pageInfo.PageRequestInfo.Filter, pageInfo.PageRequestInfo.KeyToSearch);
            var totalItems = itensFiltrados.Count;

            PaginationService<T>.CalculatePageRange(pageInfo);
            PaginationService<T>.PaginateEntities(pageInfo);

            var pageInfoDTO = new PageInfoDTO<T>()
            {
                TotalItems = totalItems,
                CurrentPage = pageInfo.CurrentPage,
                ItemsPerPage = pageInfo.ItemsPerPage,
                Entities = pageInfo.Entities,
                StartPage = pageInfo.StartPage,
                EndPage = pageInfo.EndPage,
                PageRequestInfo = pageInfo.PageRequestInfo
            };

            Page = pageInfoDTO;
            PageRequestService.ConfigurePageRequestInfo(Page.PageRequestInfo.ApiControllerName, Page.PageRequestInfo.ApiControllerAction, Page.PageRequestInfo.Filter);
        }

        private static void PaginateEntities(PageInfoDTO<T> pageInfo)
        {
            pageInfo.Entities = pageInfo.Entities
                  .Skip((pageInfo.CurrentPage - 1) * pageInfo.ItemsPerPage)
                  .Take(pageInfo.ItemsPerPage)
                  .ToList();
        }

        private static void CalculatePageRange(PageInfoDTO<T> pageInfo)
        {
            pageInfo.StartPage = pageInfo.CurrentPage - 2 > 0 ? pageInfo.CurrentPage - 2 : 1;
            pageInfo.EndPage = pageInfo.StartPage + 4 <= pageInfo.TotalPages ? pageInfo.StartPage + 4 : pageInfo.TotalPages;
        }

        public PageInfoDTO<T> GetPageInfo()
        {
            return Page;
        }

        public PageRequestInfoDTO GetPageRequestInfo()
        {
            return PageRequestService.GetPageRequestInfo();
        }

        public void ConfigurePageRequestInfo(string apiControllerName, string apiControllerAction, string filter = "")
        {
            PageRequestService.ConfigurePageRequestInfo(apiControllerName, apiControllerAction, filter);
        }
    }
}