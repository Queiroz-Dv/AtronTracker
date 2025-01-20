using Atron.WebViews.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Interfaces;
using Shared.Models;

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
            ViewData["ActionPage"] = _paginationService.GetPageRequestInfo().Action;
        }

        public void ConfigureDataTitleForView(string title)
        {
            ViewData["Title"] = title;
        }

        public void ConfigureViewBagCurrentController()
        {
            ViewBag.CurrentController = _paginationService.GetPageRequestInfo().CurrentController;
        }

        public void ConfigureViewDataFilter()
        {
            ViewData["Filter"] = _paginationService.GetPageRequestInfo().Filter;
        }

        public void CreateTempDataMessages()
        {
            TempData["Notifications"] = JsonConvert.SerializeObject(_messageModel.Messages);
        }
    }
}