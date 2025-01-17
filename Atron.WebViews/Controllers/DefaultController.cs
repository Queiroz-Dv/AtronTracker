using Atron.Domain.Extensions;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class DefaultController<DTO, Entity, ExternalService> : ServiceConteinerController<DTO, Entity, ExternalService>
    {
        protected readonly IUrlModuleFactory _urlFactory;
        private readonly IApiRouteExternalService _apiRouteExternalService;
        private IConfiguration _configuration;
        private readonly RotaDeAcesso _appSettingsConfig;

        public DefaultController(
            IUrlModuleFactory urlFactory,                     // Interface de URLs
            IPaginationService<DTO> paginationService,        // Interface de paginação
            ExternalService service,                          // Interface do serviço da entidade
            IApiRouteExternalService apiRouteExternalService, // Interface de serviços das rotas da API
            IConfiguration configuration,                     // Interface de configuração do sistema
            IOptions<RotaDeAcesso> appSettingsConfig,         // Interface que obtém as configurações do JSON
            MessageModel<Entity> messageModel)                // Modelo de serviço de mensagem 
            : base(paginationService, service, messageModel)
        {
            _urlFactory = urlFactory;
            _apiRouteExternalService = apiRouteExternalService;
            _configuration = configuration;
            _appSettingsConfig = appSettingsConfig.Value;
        }

        // Monta a rota de acordo com o modulo
        protected void BuildRoute(string controlador = "", string parametro = "")
        {
            var config = _appSettingsConfig;

            // Monta a URI do módulo
            string rota = ApiRouteBuilder.BuildRoute(config.Protocolo, config.Url);
            string url = controlador.IsNullOrEmpty() ? ApiRouteBuilder.BuildUrl(rota, CurrentController, ActionName, parametro) :
                                                       ApiRouteBuilder.BuildUrl(rota, controlador, ActionName, parametro);

            // Passa a url para a interface que será utilizada nos serviços
            _urlFactory.Url = url;
            
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

            _paginationService.Paginate(itens, itemPage, CurrentController, Filter, nameof(Index), KeyToSearch);

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

        public virtual void ConfigureCurrentPageAction(string action)
        {
            ViewData["ActionPage"] = action;
        }
    }
}