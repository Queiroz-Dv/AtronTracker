using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Enums;
using Shared.Services;

namespace Atron.WebViews.Controllers
{
    public class DefaultController<T> : Controller
    {
        private readonly PaginationService<T> PaginationService;
        public ResultResponseViewModel ResponseViewModel;
        public PageInfoDTO PageInfo { get; set; }
        public string Filter { get; set; }
        public bool ForceFilter { get; set; } = true;
        public string CurrentController { get; set; }

        public DefaultController()
        {
            ResponseViewModel = new ResultResponseViewModel();
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

    public class ResultResponseViewModel : ResultResponseViewService
    {
        public override void AddError(string message)
        {
            AddNotification(message, ResultResponseLevelEnum.Error);
        }

        public override void AddSuccess(string message)
        {
            AddNotification(message, ResultResponseLevelEnum.Success);
        }

        public override void AddWarning(string message)
        {
            AddNotification(message, ResultResponseLevelEnum.Warning);
        }
    }

    public abstract class ResultResponseViewService : IResultResponseViewService
    {
        protected ResultResponseViewService()
        {
            Messages = new List<ResultResponse>();
        }

        public void AddNotification(string message, ResultResponseLevelEnum level)
        {
            Messages.Add(new ResultResponse() { Message = message, MessageLevel = level });
        }

        public List<ResultResponse> Messages { get; set; }

        public abstract void AddSuccess(string message);

        public abstract void AddError(string message);

        public abstract void AddWarning(string message);
    }

    public interface IResultResponseViewService
    {
        List<ResultResponse> Messages { get; set; }

        void AddSuccess(string message);

        void AddError(string message);

        void AddWarning(string message);
    }
}