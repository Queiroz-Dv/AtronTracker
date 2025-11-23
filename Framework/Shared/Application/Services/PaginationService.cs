using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;

namespace Shared.Application.Services
{
    /// <summary>
    /// Classe de implementação dos serviços de paginação
    /// </summary>
    /// <typeparam name="T">Entidade que será utilizada no processo</typeparam>
    public class PaginationService<T> : IPaginationService<T>
    {
        private readonly IFilterService<T> _filterService;

        private List<T> Entities { get; set; }

        private PageInfoDTO Page { get; set; }

        public PaginationService(IFilterService<T> filterService)
        {
            _filterService = filterService;
            Page = new PageInfoDTO();
            Entities = new List<T>();
        }

        public void ConfigurePagination(List<T> itens, PageInfoDTO pageInfo)
        {
            var itensFiltrados = _filterService.ApplyFilter(itens, pageInfo.PageRequestInfo.Filter, pageInfo.PageRequestInfo.KeyToSearch);
            var totalItems = itensFiltrados.Count;

            CalculatePageRange(pageInfo);
            Entities = PaginateEntities(itensFiltrados, pageInfo);

            var pageInfoDTO = new PageInfoDTO()
            {
                TotalItems = totalItems,
                CurrentPage = pageInfo.CurrentPage,
                ItemsPerPage = pageInfo.ItemsPerPage,
                StartPage = pageInfo.StartPage,
                EndPage = pageInfo.EndPage,
                PageRequestInfo = pageInfo.PageRequestInfo
            };

            Page = pageInfoDTO;
        }

        private List<T> PaginateEntities(List<T> entities, PageInfoDTO pageInfo)
        {
            return entities
                   .Skip((pageInfo.CurrentPage - 1) * pageInfo.ItemsPerPage)
                   .Take(pageInfo.ItemsPerPage)
                   .ToList();
        }

        private static void CalculatePageRange(PageInfoDTO pageInfo)
        {
            pageInfo.StartPage = pageInfo.CurrentPage - 2 > 0 ? pageInfo.CurrentPage - 2 : 1;
            pageInfo.EndPage = pageInfo.StartPage + 4 <= pageInfo.TotalPages ? pageInfo.StartPage + 4 : pageInfo.TotalPages;
        }

        public PageInfoDTO GetPageInfo()
        {
            return Page;
        }

        public List<T> GetEntitiesFilled()
        {
            return Entities;
        }
    }
}