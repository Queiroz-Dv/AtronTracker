using Atron.WebViews.Helpers;
using Communication.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.Interfaces;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{    
    public  class MainController<DTO, Entity> : ServiceConteinerController<DTO, Entity>
    {
        protected IRouterBuilderService _router;
        protected string ApiControllerName;

        public MainController(MessageModel messageModel, IPaginationService<DTO> paginationService)
            : base(messageModel)
        {
            _paginationService = paginationService;
        }

        public override Task<IActionResult> Index(string filter = "", int itemPage = 1)
        {
            return Task.FromResult<IActionResult>(View());
        }

        public override Task<string> ObterMensagemExclusao()
        {
            return Task.FromResult("Tem certeza que deseja remover este registro ?");
        }

        // Monta a rota de acordo com o modulo
        protected void BuildRoute(string apiActionName = "", string parameter = "")
        {
            ConfigureApiInfo.ApiControllerName = ApiControllerName;
            ConfigureApiInfo.ApiActionName = apiActionName;
            ConfigureApiInfo.Parameter = parameter;
            ConfigureApiInfo.BuildMainRoute();
            ConfigureApiInfo.BuildModuleUrl();
            _router.TransferRouteToApiClient(ConfigureApiInfo.GetUrlPath());
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

            // Configurar o token na sessão
            HttpContext.Session.SetString("AuthToken", token);
        }
    }
}