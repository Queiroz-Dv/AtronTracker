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
            PageInfo = new PageInfoDTO();
            _messageModel = messageModel;
        }

        // Monta a rota de acordo com o módulo
        protected void BuildRoute(string modulo)
        {
            _apiUri.Uri = GetDefaultRoute();
            _apiUri.Modulo = modulo;
        }

        private string GetDefaultRoute()
        {
            var _config = _appSettingsConfig;
            string urlCompleta = $"{_config.Metodo}{_config.Url}/";

            // Adiciona o módulo de acesso e o nome de acesso
            urlCompleta += $"{_config.ModuloDeAcesso}/{_config.NomeDeAcesso}";

            return urlCompleta;
        }

        /// <summary>
        /// Configura a paginação e prepara as entidades para a exibição na View.
        /// </summary>
        /// <param name="itens">Lista de entidades a serem paginadas</param>
        /// <param name="itemPage">Número da página atual</param>
        protected virtual void ConfigurePaginationForView(List<T> itens, int itemPage = 1, string currentController = "", string filter = "")
        {
            ConfigureViewDataFilter();
            ConfigureViewBagCurrentController();
            ConfigurePagination(itens, itemPage, currentController, filter);
        }

        public void ConfigurePagination(List<T> itens, int itemPage, string currentController = "", string filter = "")
        {
            ProcessPagination(itens, itemPage, currentController, filter);
        }


        private void ProcessPagination(List<T> itens, int itemPage, string currentController = "", string filter = "")
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

        /// <summary>
        /// Cria as notificações de TempData a partir das mensagens de resposta.
        /// </summary>
        /// <param name="resultResponses">Lista de respostas com notificações</param>
        protected virtual void CreateTempDataNotifications(List<ResultResponseDTO> resultResponses)
        {
            var responseSerialized = JsonConvert.SerializeObject(resultResponses);
            TempData["Notifications"] = responseSerialized;
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