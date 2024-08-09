using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Services;

namespace Atron.WebViews.Controllers
{
    public class DefaultController<T> : Controller
    {
        private readonly PaginationService _paginationService;
        public List<ResultResponse> ResultResponses { get; set; }
        private List<T> Entities { get; set; }


        private PageInfoDTO PageInfoDTO { get; set; }
        public string Filter { get; set; }
        public bool ForceFilter { get; set; }
        public string CurrentController { get; set; }


        public DefaultController()
        {
            _paginationService = new PaginationService();
            ResultResponses = new List<ResultResponse>();
            Entities = new List<T>();
            PageInfoDTO = new PageInfoDTO();
        }

        public virtual void ConfigureEntitiesForView(List<T> itens, int itemPage = 1)
        {
            ViewData["Filter"] = Filter;
            var pageInfo = _paginationService.Paginate(itens, itemPage, CurrentController, Filter);

            if (!string.IsNullOrEmpty(Filter))
            {
                _paginationService.ForceFilter = ForceFilter;
                _paginationService.FilterBy = Filter;
            }

            var entitiesPaginated = _paginationService.GetEntityPaginated(itens, Filter);
            Entities = entitiesPaginated;
            PageInfoDTO = pageInfo;
        }

        public virtual void ConfigureNotifications(List<ResultResponse> resultResponses)
        {
            var responseSerialized = JsonConvert.SerializeObject(resultResponses);
            TempData["Notifications"] = responseSerialized;
        }

        public virtual List<T> GetEntities()
        {
            return Entities;
        }

        public virtual PageInfoDTO GetPageInfo()
        {
            return PageInfoDTO;
        }

        public virtual void ConfigureViewDataTitle(string title)
        {
            ViewData["Title"] = title;
        }
    }
}