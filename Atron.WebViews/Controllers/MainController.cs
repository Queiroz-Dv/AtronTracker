using Atron.WebViews.Helpers;
using Atron.WebViews.Models;
using Communication.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Shared.DTO;
using Shared.Extensions;
using Shared.Interfaces.Services;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    public class MainController<DTO, Entity> : ServiceContainerController<DTO, Entity>
    {
        protected IRouterBuilderService _router;
        protected string ApiControllerName;
        protected string KeyToSearch;

        public MainController(MessageModel messageModel, IPaginationService<DTO> paginationService)
            : base(messageModel)
        {
            _paginationService = paginationService;
            KeyToSearch = "Codigo";
        }

        public override void ConfigureViewBagCurrentController()
        {
            ViewBag.CurrentController = ApiControllerName;
        }

        public override Task<string> ObterMensagemExclusao()
        {
            return Task.FromResult("Tem certeza que deseja remover este registro ?");
        }

        // Monta a rota de acordo com o modulo
        protected void BuildRoute(string apiActionName = "", string parameter = "")
        {
            _router.TransferRouteToApiClient(ConfigureApiExtensions.ConfigureAndGetUrlPath(ApiControllerName, apiActionName, parameter));
        }

        /// <summary>
        /// Configura a paginação
        /// </summary>
        /// <param name="itens">Lista de entidades a serem paginadas</param>
        /// <param name="itemPage">Número da página atual</param>
        protected virtual void ConfigurePaginationForView(List<DTO> itens, PageInfoDTO pageInfo)
        {
            _paginationService.ConfigurePagination(itens, pageInfo);
            ConfigureViewDataFilter();
            ConfigureViewBagCurrentController();
        }

        protected virtual void ConfigurePaginationForView(List<DTO> itens, string action, int itemPage, string filter)
        {
            var pageInfo = new PageInfoDTO()
            {
                CurrentPage = itemPage,
                PageRequestInfo = new PageRequestInfoDTO()
                {
                    CurrentViewController = ApiControllerName,
                    Action = action,
                    Filter = filter,
                    KeyToSearch = KeyToSearch.IsNullOrEmpty() ? string.Empty : KeyToSearch
                }
            };

            _paginationService.ConfigurePagination(itens, pageInfo);
            ConfigureViewDataFilter();
            ConfigureViewBagCurrentController();
        }

        protected void SetAuthToken(string token, DateTime expires)
        {
            // Configurar o token nos cookies
            HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true, // Apenas acessível via HTTP
                Secure = true, // Apenas HTTPS
                SameSite = SameSiteMode.Strict, // Restringir a envio a requisições da mesma origem
                Expires = expires, // Tempo de expiração
            });
        }

        /// <summary>
        /// Cria e retorna uma nova instância do tipo de modelo especificado,
        /// populando-a com entidades e informações de paginação do serviço de paginação.
        /// </summary>
        /// <typeparam name="TModel">O tipo do modelo a ser criado. Deve herdar de DefaultModel&lt;DTO&gt; e ter um construtor sem parâmetros.</typeparam>
        /// <returns>Uma nova instância do tipo de modelo especificado, populada com entidades e informações de paginação.</returns>
        protected TModel GetModel<TModel>()
            where TModel : DefaultModel<DTO>, new()
        {
            return new TModel { Entities = _paginationService.GetEntitiesFilled(), PageInfo = _paginationService.GetPageInfo() };
        }
    }
}