using Atron.WebViews.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Interfaces;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    // Controller que será um conteiner para todos o fluxo
    public abstract class ServiceConteinerController<DTO, Entity> : Controller, IViewConfiguration
    {
        protected IPaginationService<DTO> _paginationService;

        protected MessageModel _messageModel;


        protected ServiceConteinerController(MessageModel messageModel)
        {
            _messageModel = messageModel;
        }

        public void ConfigureCurrentPageAction()
        {
            ViewData["ActionPage"] = _paginationService.GetPageInfo().PageRequestInfo.Action;
        }

        public void ConfigureCurrentPageAction(string currentPageAction)
        {
            ViewData["ActionPage"] = currentPageAction;
        }

        public void ConfigureDataTitleForView(string title)
        {
            ViewData["Title"] = title;
        }

        public void ConfigureViewBagCurrentController()
        {
            ViewBag.CurrentController = _paginationService.GetPageInfo().PageRequestInfo.CurrentViewController;
        }

        public void ConfigureViewDataFilter()
        {
            ViewData["Filter"] = _paginationService.GetPageInfo().PageRequestInfo.Filter;
        }

        public void CreateTempDataMessages()
        {
            TempData["Notifications"] = JsonConvert.SerializeObject(_messageModel.Messages);
        }

        public void AddNotificationMessage(string message)
        {
            ViewBag.NotificationMessage = message;
        }

        [HttpGet]
        public abstract Task<string> ObterMensagemExclusao();


        [HttpGet, HttpPost]
        [Route("[controller]/MenuPrincipal")]
        public abstract Task<IActionResult> Index(string filter = "", int itemPage = 1);
    }
}