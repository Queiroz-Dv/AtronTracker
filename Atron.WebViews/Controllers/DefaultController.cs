using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.DTO.API;
using Shared.Interfaces;
using Shared.Models;
using System.Collections.Generic;

namespace Atron.WebViews.Controllers
{
    // Controller que será um conteiner para todos o fluxo
    public abstract class ServiceConteinerController<DTO, Entity, ExternalService> : Controller
    {
        protected IPaginationService<DTO> _paginationService;

        protected ExternalService _service;

        protected MessageModel<Entity> _messageModel;

        protected PageInfoDTO PageInfo { get; set; }

        protected string Filter { get; set; }

        protected bool ForceFilter { get; set; } = true;

        protected string CurrentController { get; set; }

        protected ServiceConteinerController(
            IPaginationService<DTO> paginationService,
            ExternalService externalService,
            MessageModel<Entity> messageModel)
        {
            _paginationService = paginationService;
            _service = externalService;
            _messageModel = messageModel;
            PageInfo = new PageInfoDTO();
            CurrentController = nameof(Entity);
        }
    }

    public class DefaultController<DTO, Entity, ExternalService> : ServiceConteinerController<DTO, Entity, ExternalService>
    {
        protected readonly IApiUri _apiUri;
        private readonly IApiRouteExternalService _apiRouteExternalService;
        private IConfiguration _configuration;
        private readonly RotaDeAcesso _appSettingsConfig;

        public DefaultController(
            IPaginationService<DTO> paginationService,
            ExternalService service,
            IApiRouteExternalService apiRouteExternalService,
            IConfiguration configuration,
            IOptions<RotaDeAcesso> appSettingsConfig,
            MessageModel<Entity> messageModel)
            : base(paginationService, service, messageModel)
        {
            _apiRouteExternalService = apiRouteExternalService;
            _apiUri = (IApiUri)service;
            _configuration = configuration;
            _appSettingsConfig = appSettingsConfig.Value;
            CurrentController = nameof(Entity);
            BuildRoute(nameof(Entity));
        }

        // Monta a rota de acordo com o m�dulo
        protected void BuildRoute(string modulo)
        {
            _apiUri.Uri = GetDefaultRoute();
            _apiUri.Modulo = modulo;
        }

        private string GetDefaultRoute()
        {
            var _config = _appSettingsConfig;
            string urlCompleta = $"{_config.Metodo}{_config.Url}/";

            // Adiciona o m�dulo de acesso e o nome de acesso
            urlCompleta += $"{_config.ModuloDeAcesso}/{_config.NomeDeAcesso}";

            return urlCompleta;
        }

        /// <summary>
        /// Configura a paginação e prepara as entidades para a exibi��o na View.
        /// </summary>
        /// <param name="itens">Lista de entidades a serem paginadas</param>
        /// <param name="itemPage">Número da página atual</param>
        protected virtual void ConfigurePaginationForView(List<DTO> itens, int itemPage = 1, string currentController = "", string filter = "")
        {
            ConfigureViewDataFilter();
            ConfigureViewBagCurrentController();
            ConfigurePagination(itens, itemPage, currentController, filter);
        }

        public void ConfigurePagination(List<DTO> itens, int itemPage, string currentController = "", string filter = "")
        {
            ProcessPagination(itens, itemPage, currentController, filter);
        }


        private void ProcessPagination(List<DTO> itens, int itemPage, string currentController = "", string filter = "")
        {
            if (string.IsNullOrEmpty(currentController))
            {
                CurrentController = currentController;
            }

            if (string.IsNullOrEmpty(filter))
            {
                Filter = filter;
            }

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

        protected void ConfigureViewDataFilter()
        {
            ViewData["Filter"] = Filter;
        }

        protected virtual void CreateTempDataMessages()
        {
            var messagesSerialized = JsonConvert.SerializeObject(_messageModel.Messages);
            TempData["Notifications"] = messagesSerialized;
        }


        protected virtual void CreateTempDataMessages(IList<Message> messages)
        {
            var messagesSerialized = JsonConvert.SerializeObject(messages);
            TempData["Notifications"] = messagesSerialized;
        }

        /// <summary>
        /// Obtém as entidades paginadas da página atual.
        /// </summary>
        /// <returns>Lista de entidades paginadas</returns>
        protected List<DTO> GetEntitiesPaginated()
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