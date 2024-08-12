using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Interfaces;
using System.Collections.Generic;

namespace Atron.WebViews.Controllers
{
    public class DefaultController<T> : Controller
    {
        protected readonly IPaginationService<T> _paginationService;
        protected readonly IResultResponseService _responseService;

        protected PageInfoDTO PageInfo { get; set; }

        protected string Filter { get; set; }

        protected bool ForceFilter { get; set; } = true;

        protected string CurrentController { get; set; }

        public DefaultController(IPaginationService<T> paginationService, IResultResponseService responseModel)
        {
            _responseService = responseModel;
            _paginationService = paginationService;
            PageInfo = new PageInfoDTO();
        }

        /// <summary>
        /// Configura a paginação e prepara as entidades para a exibição na View.
        /// </summary>
        /// <param name="items">Lista de entidades a serem paginadas</param>
        /// <param name="itemPage">Número da página atual</param>
        protected virtual void ConfigurePaginationForView(List<T> itens, int itemPage = 1)
        {
            ConfigureViewDataFilter();
            ConfigureViewBagCurrentController();
            ConfigurePagination(itens, itemPage);
        }

        private void ConfigurePagination(List<T> itens, int itemPage)
        {
            _paginationService.Paginate(itens, itemPage, CurrentController, Filter);

            if (!string.IsNullOrEmpty(Filter))
            {
                _paginationService.ForceFilter = ForceFilter;
                _paginationService.FilterBy = Filter;
            }

            _paginationService.ConfigureEntityPaginated(itens, Filter);
            PageInfo = GetPageInfo();
        }

        private void ConfigureViewBagCurrentController()
        {
            ViewBag.CurrentController = CurrentController;
        }

        private void ConfigureViewDataFilter()
        {
            ViewData["Filter"] = Filter;
        }

        /// <summary>
        /// Cria as notificações de TempData a partir das mensagens de resposta.
        /// </summary>
        /// <param name="resultResponses">Lista de respostas com notificações</param>
        protected virtual void CreateTempDataNotifications(List<ResultResponse> resultResponses)
        {
            var responseSerialized = JsonConvert.SerializeObject(resultResponses);
            TempData["Notifications"] = responseSerialized;
        }

        /// <summary>
        /// Cria as notificações de TempData usando o modelo de resposta atual.
        /// </summary>
        protected virtual void CreateTempDataNotifications()
        {
            var resultSerialized = JsonConvert.SerializeObject(_responseService.ResultMessages);
            TempData["Notifications"] = resultSerialized;
        }

        /// <summary>
        /// Obtém as entidades paginadas da página atual.
        /// </summary>
        /// <returns>Lista de entidades paginadas</returns>
        protected List<T> GetEntitiesPaginated()
        {
            return _paginationService.GetEntitiesFilled();
        }

        private PageInfoDTO GetPageInfo()
        {
            return _paginationService.PageInfo;
        }

        /// <summary>
        /// Configura o título da View usando ViewData.
        /// </summary>
        /// <param name="title">Título da View</param>
        public virtual void ConfigureDataTitleForView(string title)
        {
            ViewData["Title"] = title;
        }
    }
}