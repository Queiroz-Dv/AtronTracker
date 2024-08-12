using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Models;
using Shared.Services;

namespace Atron.WebViews.Controllers
{
    public class DefaultController<T> : Controller
    {
        private readonly PaginationService<T> PaginationService;

        protected ResultResponseModel ResponseModel;

        protected PageInfoDTO PageInfo { get; set; }

        protected string Filter { get; set; }

        protected bool ForceFilter { get; set; } = true;

        protected string CurrentController { get; set; }

        public DefaultController(PaginationService<T> paginationService, ResultResponseModel responseModel)
        {
            ResponseModel = responseModel;
            PaginationService = paginationService;
            PageInfo = new PageInfoDTO();
        }

        protected virtual void ConfigureEntitiesForView(List<T> itens, int itemPage = 1)
        {
            ViewData["Filter"] = Filter;
            ViewBag.CurrentController = CurrentController;
            PaginationService.Paginate(itens, itemPage, CurrentController, Filter);

            if (!string.IsNullOrEmpty(Filter))
            {
                PaginationService.ForceFilter = ForceFilter;
                PaginationService.FilterBy = Filter;
            }

            PaginationService.ConfigureEntityPaginated(itens, Filter);
            PageInfo = GetPageInfo();
        }

        protected virtual void CreateTempDataNotifications(List<ResultResponse> resultResponses)
        {            
            var responseSerialized = JsonConvert.SerializeObject(resultResponses);
            TempData["Notifications"] = responseSerialized;
        }

        protected void CreateResultNotifications()
        {
            var resultSerialized = JsonConvert.SerializeObject(ResponseModel.ResultMessages);
            TempData["Notifications"] = resultSerialized;
        }

        protected List<T> GetEntities()
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