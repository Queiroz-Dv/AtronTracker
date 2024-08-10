using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Enums;
using Shared.Models;
using Shared.Services;

namespace Atron.WebViews.Controllers
{
    public class DefaultController<T> : Controller
    {
        private readonly PaginationService<T> PaginationService;
        public ResultResponseModel ResponseModel;
        public PageInfoDTO PageInfo { get; set; }
        public string Filter { get; set; }
        public bool ForceFilter { get; set; } = true;
        public string CurrentController { get; set; }

        public DefaultController()
        {
            ResponseModel = new ResultResponseModel();
            PaginationService = new PaginationService<T>();
            PageInfo = new PageInfoDTO();
        }

        public virtual void ConfigureEntitiesForView(List<T> itens, int itemPage = 1)
        {
            ViewData["Filter"] = Filter;
            PaginationService.Paginate(itens, itemPage, CurrentController, Filter);

            if (!string.IsNullOrEmpty(Filter))
            {
                PaginationService.ForceFilter = ForceFilter;
                PaginationService.FilterBy = Filter;
            }

            PaginationService.ConfigureEntityPaginated(itens, Filter);
            PageInfo = GetPageInfo();
        }

        public virtual void CreateTempDataNotifications(List<ResultResponse> resultResponses)
        {            
            var responseSerialized = JsonConvert.SerializeObject(resultResponses);
            TempData["Notifications"] = responseSerialized;
        }

        public List<T> GetEntities()
        {
            return PaginationService.GetEntitiesFilled();
        }

        private  PageInfoDTO GetPageInfo()
        {
            return PaginationService.PageInfo;
        }

        public virtual void ConfigureViewDataTitle(string title)
        {
            ViewData["Title"] = title;
        }
    }           
}