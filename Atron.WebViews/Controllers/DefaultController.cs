using Atron.Domain.ApiEntities;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.DTO.API;
using Shared.Interfaces;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class DefaultController<T> : Controller
    {
        protected readonly IPaginationService<T> _paginationService;
        protected readonly IResultResponseService _responseService;
        private readonly IApiRouteExternalService _apiRouteExternalService;
        private IConfiguration _configuration;
        private readonly RotaDeAcesso _appSettingsConfig;

        protected PageInfoDTO PageInfo { get; set; }

        protected string Filter { get; set; }

        protected bool ForceFilter { get; set; } = true;

        protected string CurrentController { get; set; }
        
        public DefaultController(
            IPaginationService<T> paginationService, 
            IResultResponseService responseModel, 
            IApiRouteExternalService apiRouteExternalService, 
            IConfiguration configuration,
            IOptions<RotaDeAcesso> appSettingsConfig)
        {
            _appSettingsConfig = appSettingsConfig.Value;
            _configuration = configuration;
            _apiRouteExternalService = apiRouteExternalService;
            _responseService = responseModel;
            _paginationService = paginationService;
            PageInfo = new PageInfoDTO();
        }

        public string ObterRotaPadrao()
        {
            var _config = _appSettingsConfig;
            string urlCompleta = $"{_config.Metodo}{_config.Url}/";

            // Adiciona o módulo de acesso e o nome de acesso
            urlCompleta += $"{_config.ModuloDeAcesso}/{_config.NomeDeAcesso}";

            return urlCompleta;
        }

        //public async Task<List<ApiRoute>> MontarRotas(string rota ,string modulo)
        //{
        //    _apiRouteExternalService.RotaDoConnect = rota;
        //    var rotas = await _apiRouteExternalService.MontarRotaDoModulo(modulo);
        //    return rotas;
        //}

        /// <summary>
        /// Configura a paginação e prepara as entidades para a exibição na View.
        /// </summary>
        /// <param name="items">Lista de entidades a serem paginadas</param>
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
            TempData["Notifications"] = messages;
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